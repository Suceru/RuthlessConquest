using System;

namespace Game
{
    internal class MusicModule : Module
    {
        public MusicModule(Game game) : base(game)
        {
        }

        public void Update()
        {
            if (StepModule.IsGameStarted)
            {
                if (StepModule.CountdownStepsLeft > 0)
                {
                    MusicManager.CurrentMix = MusicManager.Mix.None;
                    return;
                }
                MusicManager.CurrentMix = MusicManager.Mix.Game;
            }
        }
    }
}
