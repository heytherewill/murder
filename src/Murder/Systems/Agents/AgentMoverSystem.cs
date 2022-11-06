﻿using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Helpers;
using Murder.Utilities;

namespace Road.Systems
{
    /// <summary>
    /// System that looks for AgentImpulse systems and translated them into 'Velocity' for the physics system.
    /// </summary>
    [Filter(typeof(AgentComponent), typeof(AgentImpulseComponent))]
    internal class AgentMoverSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var agent = e.GetAgent();
                var impulse = e.GetAgentImpulse();

                Vector2 startVelocity = Vector2.Zero;
                if (e.TryGetVelocity() is VelocityComponent velocity)
                {
                    startVelocity = velocity.Velocity;
                }

                e.SetFacing(new FacingComponent(DirectionHelper.FromVector(impulse.Impulse)));

                // Use friction on any axis that's not receiving impulse or is receiving it in an oposing direction
                var result = GetVelocity(agent, impulse, startVelocity);

                e.RemoveFriction();     // Remove friction to move
                e.SetVelocity(result); // Turn impulse into velocity
            }

            return default;
        }

        private static Vector2 GetVelocity(AgentComponent agent, AgentImpulseComponent impulse, in Vector2 currentVelocity)
        {
            var velocity = currentVelocity;
            if (impulse.Impulse.HasValue)
            {
                if (impulse.Impulse.X == 0 || !Calculator.SameSignOrSimilar(impulse.Impulse.X, currentVelocity.X))
                {
                    velocity = new Vector2(currentVelocity.X * agent.Friction, velocity.Y);
                }
                if (impulse.Impulse.Y == 0 || !Calculator.SameSignOrSimilar(impulse.Impulse.Y, currentVelocity.Y))
                {
                    velocity = new Vector2(velocity.X, velocity.Y * agent.Friction);
                }
            }

            return Calculator.Approach(velocity, impulse.Impulse * agent.Speed, agent.Acceleration * Game.FixedDeltaTime);
        }
    }
}