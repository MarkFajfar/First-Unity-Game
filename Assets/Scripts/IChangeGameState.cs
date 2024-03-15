using System;

namespace NavajoWars
{
    public interface IChangeGameState 
    {
        public event EventHandler<GameStateObject> OnGameStateChanged;
    }
}