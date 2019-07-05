﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

namespace Effekseer.Internal
{
	public interface IEffekseerRenderer
	{
		int layer { get; set; }

		void SetVisible(bool visible);

		void CleanUp();

		CommandBuffer GetCameraCommandBuffer(Camera camera);
		
		void Render(Camera camera, int? dstID, RenderTargetIdentifier? dstIdentifier);

		void OnPostRender(Camera camera);
	}

	class UnityRendererModel : IDisposable
	{
		public ComputeBuffer VertexBuffer;
		public ComputeBuffer IndexBuffer;
		public ComputeBuffer VertexOffsets;
		public ComputeBuffer IndexOffsets;

		public List<int> IndexCounts = new List<int>();

		public List<int> vertexOffsets = new List<int>();
		public List<int> indexOffsets = new List<int>();


		public unsafe void Initialize(byte[] buffer)
		{
			int sizeEffekseerVertex = 4 * 15;

			int version = 0;
			int offset = 0;
			version = BitConverter.ToInt32(buffer, offset);
			offset += sizeof(int);

			if(version < 1)
			{
				sizeEffekseerVertex -= 4;
			}

			if (version == 2 || version >= 5)
			{
				float scale = BitConverter.ToSingle(buffer, offset);
				offset += sizeof(float);
			}

			int modelCount = BitConverter.ToInt32(buffer, offset);
			offset += sizeof(int);

			int frameCount = 0;

			if (version >= 5)
			{
				frameCount = BitConverter.ToInt32(buffer, offset);
				offset += sizeof(int);
			}
			else
			{
				frameCount = 1;
			}

			var offsetBack = offset;

			int vertexBufferCount = 0;
			int indexBufferCount = 0;

			for (int fi = 0; fi < frameCount; fi++)
			{
				vertexOffsets.Add(vertexBufferCount);
				int vertexCount = BitConverter.ToInt32(buffer, offset);
				offset += sizeof(int);

				vertexBufferCount += vertexCount;
				offset += sizeEffekseerVertex * vertexCount;

				indexOffsets.Add(indexBufferCount);

				int faceCount = BitConverter.ToInt32(buffer, offset);

				offset += sizeof(int);

				indexBufferCount += 3 * faceCount;
				offset += sizeof(int) * (3 * faceCount);

				IndexCounts.Add(3 * faceCount);
			}

			VertexBuffer = new ComputeBuffer(vertexBufferCount, sizeof(Vertex));
			IndexBuffer = new ComputeBuffer(indexBufferCount, sizeof(int));
			offset = offsetBack;

			List<Vertex> vertex = new List<Vertex>();
			List<int> index = new List<int>();

			for (int fi = 0; fi < frameCount; fi++)
			{
				int vertexCount = BitConverter.ToInt32(buffer, offset);
				offset += sizeof(int);

				if(version < 1)
				{
					fixed (byte* vs_ = &buffer[offset])
					{
						InternalVertexV0* vs = (InternalVertexV0*)vs_;

						for (int vi = 0; vi < vertexCount; vi++)
						{
							Vertex v;
							v.Position = vs[vi].Position;
							v.UV = vs[vi].UV;
							v.Normal = vs[vi].Normal;
							v.Tangent = vs[vi].Tangent;
							v.Binormal = vs[vi].Binormal;
							v.VColor.r = 1.0f;
							v.VColor.g = 1.0f;
							v.VColor.b = 1.0f;
							v.VColor.a = 1.0f;
							vertex.Add(v);
						}
					}
				}
				else
				{
					fixed (byte* vs_ = &buffer[offset])
					{
						InternalVertex* vs = (InternalVertex*)vs_;

						for (int vi = 0; vi < vertexCount; vi++)
						{
							Vertex v;
							v.Position = vs[vi].Position;
							v.UV = vs[vi].UV;
							v.Normal = vs[vi].Normal;
							v.Tangent = vs[vi].Tangent;
							v.Binormal = vs[vi].Binormal;
							v.VColor.r = vs[vi].VColor.r / 255.0f;
							v.VColor.g = vs[vi].VColor.g / 255.0f;
							v.VColor.b = vs[vi].VColor.b / 255.0f;
							v.VColor.a = vs[vi].VColor.a / 255.0f;
							vertex.Add(v);
						}
					}
				}

				offset += sizeEffekseerVertex * vertexCount;

				int faceCount = BitConverter.ToInt32(buffer, offset);
				offset += sizeof(int);

				for (int ffi = 0; ffi < faceCount; ffi++)
				{
					int f1 = BitConverter.ToInt32(buffer, offset);
					offset += sizeof(int);

					int f2 = BitConverter.ToInt32(buffer, offset);
					offset += sizeof(int);

					int f3 = BitConverter.ToInt32(buffer, offset);
					offset += sizeof(int);

					index.Add(f1);
					index.Add(f2);
					index.Add(f3);

				}

				
			}

			VertexBuffer.SetData(vertex, 0, 0, vertex.Count);
			IndexBuffer.SetData(index, 0, 0, index.Count);

			VertexOffsets = new ComputeBuffer(vertexOffsets.Count, sizeof(int));
			IndexOffsets = new ComputeBuffer(indexOffsets.Count, sizeof(int));
			VertexOffsets.SetData(vertexOffsets);
			IndexOffsets.SetData(indexOffsets);
		}

		public void Dispose()
		{
			if (VertexBuffer != null)
			{
				VertexBuffer.Dispose();
				VertexBuffer = null;
			}

			if (IndexBuffer != null)
			{
				IndexBuffer.Dispose();
				IndexBuffer = null;
			}

			if (VertexOffsets != null)
			{
				VertexOffsets.Dispose();
				VertexOffsets = null;
			}

			if (IndexOffsets != null)
			{
				IndexOffsets.Dispose();
				IndexOffsets = null;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct InternalVertex
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector3 Binormal;
		public Vector3 Tangent;
		public Vector2 UV;
		public Color32 VColor;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct InternalVertexV0
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector3 Binormal;
		public Vector3 Tangent;
		public Vector2 UV;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Vertex
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector3 Binormal;
		public Vector3 Tangent;
		public Vector2 UV;
		public Color VColor;
	}

	internal class EffekseerRendererUtils
	{
	    internal static int ScaledClamp(int value, float scale)
		{
			var v = (int)(value * scale);
			v = Math.Max(v, 1);
			v = Math.Min(v, value);
			return v;
		}

		internal static float DistortionBufferScale
		{
			get
			{
				return 1.0f;
			}
		}

		internal static bool IsDistortionEnabled
		{
			get
			{
#if UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL || UNITY_SWITCH
				return EffekseerSettings.Instance.enableDistortionMobile;
#else
				return EffekseerSettings.Instance.enableDistortion;
#endif
			}
		}
	}
	
	internal class EffekseerRendererUnity : IEffekseerRenderer
	{
		const CameraEvent cameraEvent = CameraEvent.AfterForwardAlpha;
		const int VertexSize = 36;
		const int VertexDistortionSize = 36 + 4 * 6;

		public enum AlphaBlendType : int
		{
			Opacity = 0,
			Blend = 1,
			Add = 2,
			Sub = 3,
			Mul = 4,
		}

		struct MaterialKey
		{
			public bool ZTest;
			public bool ZWrite;
			public AlphaBlendType Blend;
			public int Cull;

			public int GetKey()
			{
				return (int)Blend +
					(ZTest ? 1 : 0) << 4 +
					(ZWrite ? 1 : 0) << 5 +
					Cull << 6;
			}
		}

		class MaterialCollection
		{
			public Shader Shader;
			Dictionary<int, Material> materials = new Dictionary<int, Material>();

			public Material GetMaterial(ref MaterialKey key)
			{
				var id = key.GetKey();

				if (materials.ContainsKey(id)) return materials[id];

				var material = new Material(Shader);

				if (key.Blend == AlphaBlendType.Opacity)
				{
					material.SetFloat("_BlendSrc", (float)UnityEngine.Rendering.BlendMode.One);
					material.SetFloat("_BlendDst", (float)UnityEngine.Rendering.BlendMode.Zero);
					material.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Add);
				}
				else if (key.Blend == AlphaBlendType.Blend)
				{
					material.SetFloat("_BlendSrc", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
					material.SetFloat("_BlendDst", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					material.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Add);
				}
				else if (key.Blend == AlphaBlendType.Add)
				{
					material.SetFloat("_BlendSrc", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
					material.SetFloat("_BlendDst", (float)UnityEngine.Rendering.BlendMode.One);
					material.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Add);
				}
				else if (key.Blend == AlphaBlendType.Mul)
				{
					material.SetFloat("_BlendSrc", (float)UnityEngine.Rendering.BlendMode.Zero);
					material.SetFloat("_BlendDst", (float)UnityEngine.Rendering.BlendMode.SrcColor);
					material.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.Add);
				}
				else if (key.Blend == AlphaBlendType.Sub)
				{
					material.SetFloat("_BlendSrc", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
					material.SetFloat("_BlendDst", (float)UnityEngine.Rendering.BlendMode.One);
					material.SetFloat("_BlendOp", (float)UnityEngine.Rendering.BlendOp.ReverseSubtract);
				}

				material.SetFloat("_ZTest", key.ZTest ? (float)UnityEngine.Rendering.CompareFunction.LessEqual : (float)UnityEngine.Rendering.CompareFunction.Disabled);
				material.SetFloat("_ZWrite", key.ZWrite ? 1.0f : 0.0f);
				material.SetFloat("_Cull", key.Cull);

				materials.Add(id, material);

				return material;
			}
		}

		class MaterialPropCollection
		{
			List<MaterialPropertyBlock> materialPropBlocks = new List<MaterialPropertyBlock>();
			int materialPropBlockOffset = 0;

			public void Reset()
			{
				materialPropBlockOffset = 0;
			}

			public MaterialPropertyBlock GetNext()
			{
				if (materialPropBlockOffset >= materialPropBlocks.Count)
				{
					materialPropBlocks.Add(new MaterialPropertyBlock());
				}

				var ret = materialPropBlocks[materialPropBlockOffset];
				materialPropBlockOffset++;
				return ret;
			}
		}

		class DelayEvent
		{
			public int RestTime = 0;
			public Action Event;
		}

		private class RenderPath : IDisposable
		{
			int VertexMaxCount = 8192 * 4 * 2;
			public Camera camera;
			public CommandBuffer commandBuffer;
			public CameraEvent cameraEvent;
			public int renderId;
			public RenderTexture renderTexture;
			public ComputeBuffer computeBufferFront;
			public ComputeBuffer computeBufferBack;
			public byte[] computeBufferTemp;
			public int LifeTime = 5;

			bool isDistortionEnabled = false;

			public MaterialPropCollection materiaProps = new MaterialPropCollection();

			List<DelayEvent> delayEvents = new List<DelayEvent>();
			
			public RenderPath(Camera camera, CameraEvent cameraEvent, int renderId)
			{
				this.camera = camera;
				this.renderId = renderId;
				this.cameraEvent = cameraEvent;
			}

			public void Init(bool enableDistortion)
			{
				bool isDistortionEnabled = enableDistortion;

				// Create a command buffer that is effekseer renderer
				this.commandBuffer = new CommandBuffer();
				this.commandBuffer.name = "Effekseer Rendering";

				// register the command to a camera
				this.camera.AddCommandBuffer(this.cameraEvent, this.commandBuffer);

				if (enableDistortion)
				{
					var width = EffekseerRendererUtils.ScaledClamp(this.camera.scaledPixelWidth, EffekseerRendererUtils.DistortionBufferScale );
					var height = EffekseerRendererUtils.ScaledClamp(this.camera.scaledPixelHeight, EffekseerRendererUtils.DistortionBufferScale);

#if UNITY_IOS || UNITY_ANDROID
					RenderTextureFormat format = RenderTextureFormat.ARGB32;
#else
					RenderTextureFormat format = (this.camera.allowHDR) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
#endif
					this.renderTexture = new RenderTexture(width, height, 0, format);
	
					// HACK for ZenPhone (cannot understand)
					if(this.renderTexture == null || !this.renderTexture.Create())
					{
						this.renderTexture = null;
					}
				}

				computeBufferFront = new ComputeBuffer(VertexMaxCount, VertexSize, ComputeBufferType.Default);
				computeBufferBack = new ComputeBuffer(VertexMaxCount, VertexSize, ComputeBufferType.Default);
				computeBufferTemp = new byte[VertexMaxCount * VertexSize];
			}

			public void ReallocateComputeBuffer()
			{
				VertexMaxCount *= 2;

				var oldBufB = computeBufferBack;
				var oldBufF = computeBufferFront;

				var e = new DelayEvent();
				e.RestTime = 5;
				e.Event = () =>
				{
					oldBufB.Dispose();
					oldBufF.Dispose();
				};
				delayEvents.Add(e);

				computeBufferFront = new ComputeBuffer(VertexMaxCount, VertexSize, ComputeBufferType.Default);
				computeBufferBack = new ComputeBuffer(VertexMaxCount, VertexSize, ComputeBufferType.Default);
				computeBufferTemp = new byte[VertexMaxCount * VertexSize];
			}

			public void Dispose()
			{
				if (this.commandBuffer != null)
				{
					if (this.camera != null)
					{
						this.camera.RemoveCommandBuffer(this.cameraEvent, this.commandBuffer);
					}
					this.commandBuffer.Dispose();
					this.commandBuffer = null;
				}

				if (this.computeBufferFront != null)
				{
					this.computeBufferFront.Dispose();
					this.computeBufferFront = null;
				}

				if (this.computeBufferBack != null)
				{
					this.computeBufferBack.Dispose();
					this.computeBufferBack = null;
				}

				foreach (var e in delayEvents)
				{
					e.Event();
				}
				delayEvents.Clear();
			}

			public bool IsValid()
			{
				if (this.isDistortionEnabled != EffekseerRendererUtils.IsDistortionEnabled) return false;

				if (this.renderTexture != null)
				{
					var width = EffekseerRendererUtils.ScaledClamp(this.camera.scaledPixelWidth, EffekseerRendererUtils.DistortionBufferScale);
					var height = EffekseerRendererUtils.ScaledClamp(this.camera.scaledPixelHeight, EffekseerRendererUtils.DistortionBufferScale);

					return width == this.renderTexture.width &&
						height == this.renderTexture.height;
				}
				return true;
			}

			public void Update()
			{
				foreach(var e in delayEvents)
				{
					e.RestTime--;
					if(e.RestTime <= 0)
					{
						e.Event();
					}
				}

				delayEvents.RemoveAll(_ => _.RestTime <= 0);
			}
		};

		MaterialCollection materials = new MaterialCollection();
		MaterialCollection materialsDistortion = new MaterialCollection();
		MaterialCollection materialsModel = new MaterialCollection();
		MaterialCollection materialsModelDistortion = new MaterialCollection();

		public EffekseerRendererUnity()
		{
			materials.Shader = EffekseerSettings.Instance.standardShader;
			materialsDistortion.Shader = EffekseerSettings.Instance.standardDistortionShader;
			materialsModel.Shader = EffekseerSettings.Instance.standardModelShader;
			materialsModelDistortion.Shader = EffekseerSettings.Instance.standardModelDistortionShader;

		}

		// RenderPath per Camera
		private Dictionary<Camera, RenderPath> renderPaths = new Dictionary<Camera, RenderPath>();

		public int layer { get; set; }

		public void SetVisible(bool visible)
		{
			if (visible)
			{
				Camera.onPreCull += Render;
				Camera.onPostRender += OnPostRender;
			}
			else
			{
				Camera.onPreCull -= Render;
				Camera.onPostRender -= OnPostRender;
			}
		}

		public void CleanUp()
		{
			// dispose all render pathes
			foreach (var pair in renderPaths)
			{
				pair.Value.Dispose();
			}
			renderPaths.Clear();
		}

		public CommandBuffer GetCameraCommandBuffer(Camera camera)
		{
			if (renderPaths.ContainsKey(camera))
			{
				return renderPaths[camera].commandBuffer;
			}
			return null;
		}

		public void Render(Camera camera)
		{
			Render(camera, null, null);
		}

		public void Render(Camera camera, int? dstID, RenderTargetIdentifier? dstIdentifier)
		{
			var settings = EffekseerSettings.Instance;

#if UNITY_EDITOR
			if (camera.cameraType == CameraType.SceneView)
			{
				// check a camera in the scene view
				if (settings.drawInSceneView == false)
				{
					return;
				}
			}
#endif
			// GC renderpaths
			bool hasDisposed = false;
			foreach (var path_ in renderPaths)
			{
				path_.Value.LifeTime--;
				if (path_.Value.LifeTime < 0)
				{
					path_.Value.Dispose();
					hasDisposed = true;
				}
			}

			if (hasDisposed)
			{
				List<Camera> removed = new List<Camera>();
				foreach (var path_ in renderPaths)
				{
					if (path_.Value.LifeTime >= 0) continue;

					removed.Add(path_.Key);
				}

				foreach(var r in removed)
				{
					renderPaths.Remove(r);
				}
			}

			RenderPath path;

			// check a culling mask
			if ((camera.cullingMask & (1 << layer)) == 0)
			{
				if (renderPaths.ContainsKey(camera))
				{
					path = renderPaths[camera];
					path.Dispose();
					renderPaths.Remove(camera);
				}
				return;
			}

			if (renderPaths.ContainsKey(camera))
			{
				path = renderPaths[camera];
			}
			else
			{
				// render path doesn't exists, create a render path
				path = new RenderPath(camera, cameraEvent, renderPaths.Count);
				path.Init(EffekseerRendererUtils.IsDistortionEnabled);
				renderPaths.Add(camera, path);
			}

			if (!path.IsValid())
			{
				path.Dispose();
				path.Init(EffekseerRendererUtils.IsDistortionEnabled);
			}

			path.Update();
			path.LifeTime = 5;

			// assign a dinsotrion texture
			if (path.renderTexture)
			{
				Plugin.EffekseerSetBackGroundTexture(path.renderId, path.renderTexture.GetNativeTexturePtr());
			}

			// specify matrixes for stereo rendering
			if (camera.stereoEnabled)
			{
				float[] projMatL = Utility.Matrix2Array(GL.GetGPUProjectionMatrix(camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left), false));
				float[] projMatR = Utility.Matrix2Array(GL.GetGPUProjectionMatrix(camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right), false));
				float[] camMatL = Utility.Matrix2Array(camera.GetStereoViewMatrix(Camera.StereoscopicEye.Left));
				float[] camMatR = Utility.Matrix2Array(camera.GetStereoViewMatrix(Camera.StereoscopicEye.Right));
				Plugin.EffekseerSetStereoRenderingMatrix(path.renderId, projMatL, projMatR, camMatL, camMatR);
			}
			else
			{
				// update view matrixes
				Plugin.EffekseerSetProjectionMatrix(path.renderId, Utility.Matrix2Array(
					GL.GetGPUProjectionMatrix(camera.projectionMatrix, false)));
				Plugin.EffekseerSetCameraMatrix(path.renderId, Utility.Matrix2Array(
					camera.worldToCameraMatrix));
			}

			// Reset command buffer
			path.commandBuffer.Clear();
			path.materiaProps.Reset();

			// generate render events on this thread
			Plugin.EffekseerRenderBack(path.renderId);

			// if memory is lacked, reallocate memory
			while(Plugin.GetUnityRenderParameterCount() > 0 && Plugin.GetUnityRenderVertexBufferCount() > path.computeBufferTemp.Length)
			{
				path.ReallocateComputeBuffer();
			}

			RenderInternal(path.commandBuffer, path.computeBufferTemp, path.computeBufferBack, path.materiaProps, path.renderTexture);

			// Distortion
			if (EffekseerRendererUtils.IsDistortionEnabled && 
				(path.renderTexture != null || dstID.HasValue || dstIdentifier.HasValue))
			{
				// Add a blit command that copy to the distortion texture
				if(dstID.HasValue)
				{
					path.commandBuffer.Blit(dstID.Value, path.renderTexture);
					path.commandBuffer.SetRenderTarget(dstID.Value);
				}
                else if (dstIdentifier.HasValue)
                {
                    path.commandBuffer.Blit(dstIdentifier.Value, path.renderTexture);
                    path.commandBuffer.SetRenderTarget(dstIdentifier.Value);
                }
				else
				{
					path.commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, path.renderTexture);
					path.commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
				}
			}

			Plugin.EffekseerRenderFront(path.renderId);

			// if memory is lacked, reallocate memory
			while (Plugin.GetUnityRenderParameterCount() > 0 && Plugin.GetUnityRenderVertexBufferCount() > path.computeBufferTemp.Length)
			{
				path.ReallocateComputeBuffer();
			}

			RenderInternal(path.commandBuffer, path.computeBufferTemp, path.computeBufferFront, path.materiaProps, path.renderTexture);
		}

		Texture GetCachedTexture(IntPtr key, RenderTexture background)
		{
			if (background != null && background.GetNativeTexturePtr() == key) return background;

			return EffekseerSystem.GetCachedTexture(key);
		}

		unsafe void RenderInternal(CommandBuffer commandBuffer, byte[] computeBufferTemp, ComputeBuffer computeBuffer, MaterialPropCollection matPropCol, RenderTexture background)
		{
			var renderParameterCount = Plugin.GetUnityRenderParameterCount();
			var vertexBufferSize = Plugin.GetUnityRenderVertexBufferCount();

			if (renderParameterCount > 0)
			{
				Plugin.UnityRenderParameter parameter = new Plugin.UnityRenderParameter();

				var vertexBufferCount = Plugin.GetUnityRenderVertexBufferCount();

				if(vertexBufferCount > 0)
				{
					var vertexBuffer = Plugin.GetUnityRenderVertexBuffer();

					System.Runtime.InteropServices.Marshal.Copy(vertexBuffer, computeBufferTemp, 0, vertexBufferCount);
					computeBuffer.SetData(computeBufferTemp, 0, 0, vertexBufferCount);
				}

				for (int i = 0; i < renderParameterCount; i++)
				{
					Plugin.GetUnityRenderParameter(ref parameter, i);
					
					if(parameter.RenderMode == 1)
					{
						// Draw model
						var infoBuffer = Plugin.GetUnityRenderInfoBuffer();
						var modelParameters = ((Plugin.UnityRenderModelParameter*)(((byte*)infoBuffer.ToPointer()) + parameter.VertexBufferOffset));

						MaterialKey key = new MaterialKey();
						key.Blend = (AlphaBlendType)parameter.Blend;
						key.ZTest = parameter.ZTest > 0;
						key.ZWrite = parameter.ZWrite > 0;

						if(parameter.Culling == 0)
						{
							key.Cull = (int)UnityEngine.Rendering.CullMode.Back;
						}
						if (parameter.Culling == 1)
						{
							key.Cull = (int)UnityEngine.Rendering.CullMode.Front;
						}
						if (parameter.Culling == 2)
						{
							key.Cull = (int)UnityEngine.Rendering.CullMode.Off;
						}

						
						for(int mi = 0; mi < parameter.ElementCount; mi++)
						{
							var model = EffekseerSystem.GetCachedModel(parameter.ModelPtr);
							if (model == null)
								continue;

							var prop = matPropCol.GetNext();

							if (parameter.IsDistortingMode > 0)
							{
								var material = materialsModelDistortion.GetMaterial(ref key);

								prop.SetBuffer("buf_vertex", model.VertexBuffer);
								prop.SetBuffer("buf_index", model.IndexBuffer);
								prop.SetMatrix("buf_matrix", modelParameters[mi].Matrix);
								prop.SetVector("buf_uv", modelParameters[mi].UV);
								prop.SetVector("buf_color", modelParameters[mi].VColor);
								prop.SetFloat("buf_vertex_offset", model.vertexOffsets[modelParameters[mi].Time]);
								prop.SetFloat("buf_index_offset", model.indexOffsets[modelParameters[mi].Time]);
								
								var colorTexture = GetCachedTexture(parameter.TexturePtrs0, background);
								if (parameter.TextureWrapTypes[0] == 0)
								{
									colorTexture.wrapMode = TextureWrapMode.Repeat;
								}
								else
								{
									colorTexture.wrapMode = TextureWrapMode.Clamp;
								}

								if (parameter.TextureFilterTypes[0] == 0)
								{
									colorTexture.filterMode = FilterMode.Point;
								}
								else
								{
									colorTexture.filterMode = FilterMode.Bilinear;
								}

								prop.SetTexture("_ColorTex", colorTexture);
								//prop.SetTexture("_BackTex", GetCachedTexture(parameter.TexturePtrs1, background));
								//Temp
								prop.SetTexture("_BackTex", background);

								commandBuffer.DrawProcedural(new Matrix4x4(), material, 0, MeshTopology.Triangles, model.IndexCounts[0], 1, prop);
							}
							else
							{
								var material = materialsModel.GetMaterial(ref key);

								prop.SetBuffer("buf_vertex", model.VertexBuffer);
								prop.SetBuffer("buf_index", model.IndexBuffer);
								prop.SetMatrix("buf_matrix", modelParameters[mi].Matrix);
								prop.SetVector("buf_uv", modelParameters[mi].UV);
								prop.SetVector("buf_color", modelParameters[mi].VColor);

								int modelTime = modelParameters[mi].Time;
								prop.SetFloat("buf_vertex_offset", model.vertexOffsets[modelTime]);
								prop.SetFloat("buf_index_offset", model.indexOffsets[modelTime]);

								var colorTexture = GetCachedTexture(parameter.TexturePtrs0, background);
								if(parameter.TextureWrapTypes[0] == 0)
								{
									colorTexture.wrapMode = TextureWrapMode.Repeat;
								}
								else
								{
									colorTexture.wrapMode = TextureWrapMode.Clamp;
								}

								if (parameter.TextureFilterTypes[0] == 0)
								{
									colorTexture.filterMode = FilterMode.Point;
								}
								else
								{
									colorTexture.filterMode = FilterMode.Bilinear;
								}

								prop.SetTexture("_ColorTex", GetCachedTexture(parameter.TexturePtrs0, background));

								commandBuffer.DrawProcedural(new Matrix4x4(), material, 0, MeshTopology.Triangles, model.IndexCounts[0], 1, prop);
							}
						}
					}
					else
					{
						var prop = matPropCol.GetNext();

						MaterialKey key = new MaterialKey();
						key.Blend = (AlphaBlendType)parameter.Blend;
						key.ZTest = parameter.ZTest > 0;
						key.ZWrite = parameter.ZWrite > 0;
						key.Cull = (int)UnityEngine.Rendering.CullMode.Off;
						

						if (parameter.IsDistortingMode > 0)
						{
							var material = materialsDistortion.GetMaterial(ref key);

							prop.SetFloat("buf_offset", parameter.VertexBufferOffset / VertexDistortionSize);
							prop.SetBuffer("buf_vertex", computeBuffer);

							var colorTexture = GetCachedTexture(parameter.TexturePtrs0, background);
							if (parameter.TextureWrapTypes[0] == 0)
							{
								colorTexture.wrapMode = TextureWrapMode.Repeat;
							}
							else
							{
								colorTexture.wrapMode = TextureWrapMode.Clamp;
							}

							if (parameter.TextureFilterTypes[0] == 0)
							{
								colorTexture.filterMode = FilterMode.Point;
							}
							else
							{
								colorTexture.filterMode = FilterMode.Bilinear;
							}

							prop.SetTexture("_ColorTex", colorTexture);
							//prop.SetTexture("_BackTex", GetCachedTexture(parameter.TexturePtrs1, background));
							//Temp
							prop.SetTexture("_BackTex", background);

							commandBuffer.DrawProcedural(new Matrix4x4(), material, 0, MeshTopology.Triangles, parameter.ElementCount * 2 * 3, 1, prop);
						}
						else
						{
							var material = materials.GetMaterial(ref key);

							prop.SetFloat("buf_offset", parameter.VertexBufferOffset / VertexSize);
							prop.SetBuffer("buf_vertex", computeBuffer);

							var colorTexture = GetCachedTexture(parameter.TexturePtrs0, background);
							if (parameter.TextureWrapTypes[0] == 0)
							{
								colorTexture.wrapMode = TextureWrapMode.Repeat;
							}
							else
							{
								colorTexture.wrapMode = TextureWrapMode.Clamp;
							}

							if (parameter.TextureFilterTypes[0] == 0)
							{
								colorTexture.filterMode = FilterMode.Point;
							}
							else
							{
								colorTexture.filterMode = FilterMode.Bilinear;
							}

							prop.SetTexture("_ColorTex", colorTexture);

							commandBuffer.DrawProcedural(new Matrix4x4(), material, 0, MeshTopology.Triangles, parameter.ElementCount * 2 * 3, 1, prop);
						}
					}
				}
			}

		}

		public void OnPostRender(Camera camera)
		{
			if (renderPaths.ContainsKey(camera))
			{
				RenderPath path = renderPaths[camera];
				Plugin.EffekseerSetRenderSettings(path.renderId,
					(camera.activeTexture != null));
			}
		}
	}

	internal class EffekseerRendererNative : IEffekseerRenderer
	{
		const CameraEvent cameraEvent = CameraEvent.AfterForwardAlpha;

		private class RenderPath : IDisposable
		{
			public Camera camera;
			public CommandBuffer commandBuffer;
			public CameraEvent cameraEvent;
			public int renderId;
			public RenderTexture renderTexture;
			public int LifeTime = 5;

			bool isDistortionEnabled = false;

			public RenderPath(Camera camera, CameraEvent cameraEvent, int renderId)
			{
				this.camera = camera;
				this.renderId = renderId;
				this.cameraEvent = cameraEvent;
			}

			public void Init(bool enableDistortion, int? dstID, RenderTargetIdentifier? dstIdentifier)
			{
				this.isDistortionEnabled = enableDistortion;

				// Create a command buffer that is effekseer renderer
				this.commandBuffer = new CommandBuffer();
				this.commandBuffer.name = "Effekseer Rendering";

				// add a command to render effects.
				this.commandBuffer.IssuePluginEvent(Plugin.EffekseerGetRenderBackFunc(), this.renderId);

				if (enableDistortion)
				{
					var width = EffekseerRendererUtils.ScaledClamp(this.camera.scaledPixelWidth, EffekseerRendererUtils.DistortionBufferScale);
					var height = EffekseerRendererUtils.ScaledClamp(this.camera.scaledPixelHeight, EffekseerRendererUtils.DistortionBufferScale);

#if UNITY_IOS || UNITY_ANDROID
					RenderTextureFormat format = RenderTextureFormat.ARGB32;
#else
					RenderTextureFormat format = (this.camera.allowHDR) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
#endif
					this.renderTexture = new RenderTexture(width, height, 0, format);

					// HACK for ZenPhone (cannot understand)
					if (this.renderTexture == null || !this.renderTexture.Create())
					{
						this.renderTexture = null;
						this.commandBuffer.IssuePluginEvent(Plugin.EffekseerGetRenderFrontFunc(), this.renderId);
						this.camera.AddCommandBuffer(this.cameraEvent, this.commandBuffer);
						return;
					}

					// Add a blit command that copy to the distortion texture
					this.commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, this.renderTexture);
					this.commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);

                    if (dstID.HasValue)
                    {
                        this.commandBuffer.Blit(dstID.Value, this.renderTexture);
                        this.commandBuffer.SetRenderTarget(dstID.Value);
                    }
                    else if (dstIdentifier.HasValue)
                    {
                        this.commandBuffer.Blit(dstIdentifier.Value, this.renderTexture);
                        this.commandBuffer.SetRenderTarget(dstIdentifier.Value);
                    }
                    else
                    {
                        this.commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, this.renderTexture);
                        this.commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                    }
                }

				this.commandBuffer.IssuePluginEvent(Plugin.EffekseerGetRenderFrontFunc(), this.renderId);

				// register the command to a camera
				this.camera.AddCommandBuffer(this.cameraEvent, this.commandBuffer);
			}

			public void Dispose()
			{
				if (this.commandBuffer != null)
				{
					if (this.camera != null)
					{
						this.camera.RemoveCommandBuffer(this.cameraEvent, this.commandBuffer);
					}
					this.commandBuffer.Dispose();
					this.commandBuffer = null;
				}
			}

			public bool IsValid()
			{
				if (this.isDistortionEnabled != EffekseerRendererUtils.IsDistortionEnabled) return false;

				if (this.renderTexture != null)
				{
					var width = EffekseerRendererUtils.ScaledClamp(this.camera.scaledPixelWidth, EffekseerRendererUtils.DistortionBufferScale);
					var height = EffekseerRendererUtils.ScaledClamp(this.camera.scaledPixelHeight, EffekseerRendererUtils.DistortionBufferScale);

					return width == this.renderTexture.width &&
						height == this.renderTexture.height;
				}
				return true;
			}
		};

		// RenderPath per Camera
		private Dictionary<Camera, RenderPath> renderPaths = new Dictionary<Camera, RenderPath>();

		public int layer { get; set; }

		public void SetVisible(bool visible)
		{
			if (visible)
			{
				Camera.onPreCull += Render;
				Camera.onPostRender += OnPostRender;
			}
			else
			{
				Camera.onPreCull -= Render;
				Camera.onPostRender -= OnPostRender;
			}
		}

		public void CleanUp()
		{
			// レンダーパスの全破棄
			foreach (var pair in renderPaths)
			{
				pair.Value.Dispose();
			}
			renderPaths.Clear();
		}

		public CommandBuffer GetCameraCommandBuffer(Camera camera)
		{
			if (renderPaths.ContainsKey(camera))
			{
				return renderPaths[camera].commandBuffer;
			}
			return null;
		}

		public void Render(Camera camera)
		{
			Render(camera, null, null);
		}

		public void Render(Camera camera, int? dstID, RenderTargetIdentifier? dstIdentifier)
		{
			var settings = EffekseerSettings.Instance;

#if UNITY_EDITOR
			if (camera.cameraType == CameraType.SceneView)
			{
				// check a camera in the scene view
				if (settings.drawInSceneView == false)
				{
					return;
				}
			}
#endif
			// GC renderpaths
			bool hasDisposed = false;
			foreach (var path_ in renderPaths)
			{
				path_.Value.LifeTime--;
				if (path_.Value.LifeTime < 0)
				{
					path_.Value.Dispose();
					hasDisposed = true;
				}
			}

			if (hasDisposed)
			{
				List<Camera> removed = new List<Camera>();
				foreach (var path_ in renderPaths)
				{
					if (path_.Value.LifeTime >= 0) continue;

					removed.Add(path_.Key);
				}

				foreach (var r in removed)
				{
					renderPaths.Remove(r);
				}
			}

			RenderPath path;

			// check a culling mask
			if ((camera.cullingMask & (1 << layer)) == 0)
			{
				if (renderPaths.ContainsKey(camera))
				{
					path = renderPaths[camera];
					path.Dispose();
					renderPaths.Remove(camera);
				}
				return;
			}

			if (renderPaths.ContainsKey(camera))
			{
				path = renderPaths[camera];
			}
			else
			{
				// render path doesn't exists, create a render path
				path = new RenderPath(camera, cameraEvent, renderPaths.Count);
				path.Init(EffekseerRendererUtils.IsDistortionEnabled, dstID, dstIdentifier);
				renderPaths.Add(camera, path);
			}

			if (!path.IsValid())
			{
				path.Dispose();
				path.Init(EffekseerRendererUtils.IsDistortionEnabled, dstID, dstIdentifier);
			}

			path.LifeTime = 5;

            // if LWRP
            if(dstID.HasValue || dstIdentifier.HasValue)
            {
				// flip a rendertaget
				// Direct11 : OK (2019, LWRP 5.13)
				// Android(OpenGL) : OK (2019, LWRP 5.13)
				Plugin.EffekseerSetRenderSettings(path.renderId, true);
                Plugin.EffekseerSetIsBackgroundTextureFlipped(0);
            }
			else
			{
				Plugin.EffekseerSetIsBackgroundTextureFlipped(0);
			}

			// assign a dinsotrion texture
			if (path.renderTexture)
			{
				Plugin.EffekseerSetBackGroundTexture(path.renderId, path.renderTexture.GetNativeTexturePtr());
			}

			// specify matrixes for stereo rendering
			if (camera.stereoEnabled)
			{
				float[] projMatL = Utility.Matrix2Array(GL.GetGPUProjectionMatrix(camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left), false));
				float[] projMatR = Utility.Matrix2Array(GL.GetGPUProjectionMatrix(camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right), false));
				float[] camMatL = Utility.Matrix2Array(camera.GetStereoViewMatrix(Camera.StereoscopicEye.Left));
				float[] camMatR = Utility.Matrix2Array(camera.GetStereoViewMatrix(Camera.StereoscopicEye.Right));
				Plugin.EffekseerSetStereoRenderingMatrix(path.renderId, projMatL, projMatR, camMatL, camMatR);
			}
			else
			{
				// update view matrixes
				Plugin.EffekseerSetProjectionMatrix(path.renderId, Utility.Matrix2Array(
					GL.GetGPUProjectionMatrix(camera.projectionMatrix, false)));
				Plugin.EffekseerSetCameraMatrix(path.renderId, Utility.Matrix2Array(
					camera.worldToCameraMatrix));
			}
		}

		public void OnPostRender(Camera camera)
		{
			if (renderPaths.ContainsKey(camera))
			{
				RenderPath path = renderPaths[camera];
				Plugin.EffekseerSetRenderSettings(path.renderId,
					(camera.activeTexture != null));
			}
		}
	}

}