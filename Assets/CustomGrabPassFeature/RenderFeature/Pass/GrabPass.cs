using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrabPass : ScriptableRenderPass
    {
        private const string PASS_NAME = "CustomGrabPass";
        
        private RenderTargetIdentifier _source;
        private RenderTargetHandle _destination;
        
        private Downsampling _downsamplingMethod;
        private string _globalProperty;
        
        public GrabPass(RenderPassEvent passEvent, string globalProperty, Downsampling downsampling)
        {
            renderPassEvent = passEvent;
            _globalProperty = globalProperty;
            _downsamplingMethod = downsampling;
            
            _destination.Init(_globalProperty);
        }

        public void Setup(RenderTargetIdentifier source)
        {
            ConfigureInput(ScriptableRenderPassInput.Color);
            
            _source = source;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor descriptor = cameraTextureDescriptor;
            
            // DownSampling 처리
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;
            if (_downsamplingMethod == Downsampling._2xBilinear)
            {
                descriptor.width /= 2;
                descriptor.height /= 2;
            }
            else if (_downsamplingMethod == Downsampling._4xBox || _downsamplingMethod == Downsampling._4xBilinear)
            {
                descriptor.width /= 4;
                descriptor.height /= 4;
            }

            // 임시 RT 생성
            // RendererData에서 Intermediate Texture옵션을 Always로 해야함.
            cmd.GetTemporaryRT(_destination.id, descriptor, _downsamplingMethod == Downsampling.None ? FilterMode.Point : FilterMode.Bilinear);
            // Global Shader Propertie 등록
            cmd.SetGlobalTexture(_globalProperty, _destination.Identifier());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(PASS_NAME);
           // context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            Blit(cmd, _source, _destination.Identifier());
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_destination.id);
        }
    }
    
    
    