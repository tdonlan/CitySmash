using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class GameConfig
    {
        public static int bulletDamage = 10;
        public static int tankBulletDamage = 50;

        public static float CameraShakeAmountSmall = .2f;
        public static float CameraShakeAmountMed = .5f;
        public static float CameraShakeAmountLarge = 1f;


        public static float soldierSpawnPercent = .5f;
        public static float tankSpawnPercent = .3f;
        public static float heliSpawnPercent = .3f;

        public static float screamPercent = .5f;
        public static float bulletPercent = .5f;

        public static int manXP = 1;
        public static int soldierXP = 5;
        public static int tankXP = 100;
        public static int heliXP = 200;
    }
}
