namespace Game.Common
{
    public static class Constants
    {        
        public static class GameState
        {
            public const string MENU_STATE_KEY = "Menu";
            public const string ROOM_STATE_KEY = "Room";
        }

        public static class MenuState
        {
            public const string SUBSTATE_ARG_NAME = "Substate";
            public const string START_SUBSTATE_KEY = "Start";
            public const string GAMEOVER_SUBSTATE_KEY = "GameOver";
        }
    }
}