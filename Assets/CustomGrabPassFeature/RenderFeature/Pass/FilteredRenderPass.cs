using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FilteredRenderPass : ScriptableRenderPass
{
    private List<ShaderTagId> _shaderTagIdList;

    private FilteringSettings _filteringSettings;
    private RenderStateBlock _renderStateBlock;

    public FilteredRenderPass(RenderPassEvent passEvent, List<ShaderTagId> shaderTagIds, LayerMask layerMask)
    {
        renderPassEvent = passEvent;
        // 특정 LightMode Tag를 가진 쉐이더만 렌더링
        _shaderTagIdList = shaderTagIds;
        // 렌더큐범위 & 레이어마스크 렌더 필터링 등록
        _filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
        // 스텐실 & 컬러마스크 등 렌더링할때 상태 세팅
        _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        ConfigureInput(ScriptableRenderPassInput.Color);
        CommandBuffer cmd = CommandBufferPool.Get();
       // context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        DrawingSettings drawSettings;
        drawSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, SortingCriteria.CommonTransparent);
        context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _filteringSettings, ref _renderStateBlock);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}