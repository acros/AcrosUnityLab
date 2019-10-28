using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acros
{
    public static class EventCategory
    {
        [EventCategoryID("(Empty)")]
        public static class Empty
        {
            //A must have value
            public const int INDEX = -1;

            [EventMemberID(INDEX, "None")]
            public const int None = 0;
        }

        [EventCategoryID("Environment")]
        public static class Environment
        {
            public const int INDEX = 0;

            [EventMemberID(INDEX, "App Pause")]
            public const int OnPause = 0;

            [EventMemberID(INDEX, "App Resume")]
            public const int OnResume = 1;
        }

        [EventCategoryID("GameSytem")]
        public static class GameSystem
        {
            public const int INDEX = 1;

            [EventMemberID(INDEX, "Game Start")]
            public const int OnGameStart = 0;

            [EventMemberID(INDEX, "Game Finish")]
            public const int OnGameFinish = 1;
        }

    }

}