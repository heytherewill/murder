﻿using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;

namespace Murder.Core;

public class Mask2D : IDisposable
{
    public readonly Vector2 Size;

    private readonly RenderTarget2D _renderTarget;
    private readonly Batch2D _batch;
    private readonly Color _color;

    public Mask2D(Vector2 size, Color? color = null)
    {
        _renderTarget = new RenderTarget2D(Game.GraphicsDevice, (int)size.X, (int)size.Y);
        _batch = new Batch2D(Game.GraphicsDevice);
        _color = color ?? Color.Transparent;

        Size = size;
    }

    public Batch2D Begin(bool debug = false)
    {
        Game.GraphicsDevice.SetRenderTarget(_renderTarget);
        Game.GraphicsDevice.Clear(_color);
        _batch.Begin(
                Game.Data.ShaderSprite,
                batchMode: BatchMode.DepthSortDescending,
                depthStencil: DepthStencilState.None,
                sampler: SamplerState.PointClamp,
                transform: Microsoft.Xna.Framework.Matrix.Identity,
                blendState: BlendState.AlphaBlend
                );
        if (debug)
        {
            _batch.DrawRectangleOutline(_renderTarget.Bounds, Color.Red);
        }
        return _batch;
    }
    public void End(Batch2D targetBatch, Vector2 position, Vector2 camera, DrawInfo drawInfo)
    {
        _batch.SetTransform(camera);
        End(targetBatch, position, drawInfo);
    }
    public void End(Batch2D targetBatch, Vector2 position, DrawInfo drawInfo)
    {
        _batch.End();
        targetBatch.Draw(_renderTarget, position, _renderTarget.Bounds.Size.ToVector2(), _renderTarget.Bounds, drawInfo.Sort,
            drawInfo.Rotation, drawInfo.Scale, drawInfo.FlippedHorizontal ? ImageFlip.Horizontal : ImageFlip.None, drawInfo.Color,
            drawInfo.Origin, drawInfo.GetBlendMode());
    }

    public void Dispose()
    {
        _renderTarget.Dispose();
        _batch.Dispose();
    }
}