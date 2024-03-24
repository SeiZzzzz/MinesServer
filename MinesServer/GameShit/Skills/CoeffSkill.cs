using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Skills
{
    internal class CoeffSkill
    {
        public static float Digcoast(float x)
        {
            float coast = 1000;
            if (x > 1 && x <= 5) { coast = coast + x * 150; }
            if (x > 5 && x <= 10) { coast = coast + (x * 250) - 50; }
            if (x > 10 && x <= 25) { coast = coast + (x * 1250) - 50; }
            if (x > 25 && x <= 50) { coast = coast + (x * 3000) - 50; }
            if (x > 50 && x <= 100) { coast = coast + (x * 7500) - 50; }
            if (x > 100 && x <= 200) { coast = coast + (x * 10000) - 50; }
            if (x > 200 && x <= 500) { coast = coast + (x * 20000) - 50; }
            if (x > 500 && x <= 600) { coast = coast + (x * 25000) - 50; }
            if (x > 600) { coast = 15000000; }
            return coast;
        }
        public static float Digeffect(float x)
        {
            float eff = 100f;
            if (x > 1) eff += x * 10;
            return eff;
        }
        public static float Digexp(float x)
        {
            float exp = 20;
            if (x > 1 && x <= 5) { exp = exp + x * 5; }
            if (x > 5 && x <= 10) { exp = exp + (x * 8) - 20; }
            if (x > 10 && x <= 25) { exp = exp + (x * 13) - 20; }
            if (x > 25 && x <= 75) { exp = exp + (x * 25) - 20; }
            if (x > 75 && x <= 200) { exp = exp + (x * 32) - 20; }
            if (x > 200 && x <= 1000) { exp = exp + (x * 34) - 20; }
            if (x > 1000) { exp = 35000; }
            return exp;
        }
        public static float Hpcoast(float x)
        {
            float coast = 50;
            if (x > 1 && x <= 5) { coast = coast + x * 150; }
            if (x > 5 && x <= 10) { coast = coast + (x * 250) - 50; }
            if (x > 10 && x <= 25) { coast = coast + (x * 2250) - 50; }
            if (x > 25 && x <= 50) { coast = coast + (x * 5000) - 50; }
            if (x > 50 && x <= 100) { coast = coast + (x * 12500) - 50; }
            if (x > 100 && x <= 200) { coast = coast + (x * 13000) - 50; }
            if (x > 200 && x <= 500) { coast = coast + (x * 17000) - 50; }
            if (x > 500 && x <= 1000) { coast = coast + (x * 25000) - 50; }
            if (x > 1000) { coast = 10000000; }
            return coast;
        }
        public static float Hpeffect(float x)
        {
            float eff = 0;
            if (x > 1 && x <= 5) { eff = eff + x * 6; }
            if (x > 5 && x <= 10) { eff = eff + x * 5; }
            if (x > 10 && x <= 50) { eff = eff + x * 4; }
            if (x > 50 && x <= 100) { eff = eff + x * 3; }
            if (x > 100 && x <= 500) { eff = eff + x * 2; }
            if (x > 500) { eff = eff + x * 1; }
            return eff;
        }
        public static float Hpexp(float x)
        {
            float exp = 25;
            if (x > 1 && x <= 5) { exp = exp + x * 5; }
            if (x > 5 && x <= 10) { exp = exp + (x * 10) - 50; }
            if (x > 10 && x <= 25) { exp = exp + (x * 20) - 50; }
            if (x > 25 && x <= 50) { exp = exp + (x * 22) - 50; }
            if (x > 50 && x <= 100) { exp = exp + (x * 25) - 50; }
            if (x > 100 && x <= 250) { exp = exp + (x * 30) - 50; }
            if (x > 250 && x <= 500) { exp = exp + (x * 40) - 50; }
            if (x > 500 && x <= 1000) { exp = exp + (x * 45) - 50; }
            if (x > 1000) { exp = 45000; }
            return exp;
        }
        public static float Minecoast(float x)
        {
            float coast = 250;
            if (x > 1 && x <= 5) { coast = coast + x * 50; }
            if (x > 5 && x <= 10) { coast = coast + (x * 250) - 250; }
            if (x > 10 && x <= 20) { coast = coast + (x * 1000) - 250; }
            if (x > 20 && x <= 50) { coast = coast + (x * 4500) - 250; }
            if (x > 50 && x <= 100) { coast = coast + (x * 12500) - 250; }
            if (x > 100 && x <= 250) { coast = coast + (x * 17500) - 250; }
            if (x > 250 && x <= 500) { coast = coast + 2375000 + (x * 10000) - 250; }
            if (x > 500 && x <= 1000) { coast = coast + 5575000 + (x * 7750) - 250; }
            if (x > 1000) { coast = 14000000; }
            return coast;
        }
        public static float Mineeffect(float x)
        {
            float eff = 1f;
            if (x > 1 && x <= 5) { eff = eff + (x * 0.15f); }
            if (x > 5 && x <= 10) { eff = eff + 0.25f + (x * 0.075f); }
            if (x > 10 && x <= 100) { eff = eff + 0.395f + (x * 0.055f); }
            if (x > 100 && x <= 300) { eff = eff + 1.365f + (x * 0.045f); }
            if (x > 300 && x <= 500) { eff = eff + 6.465f + (x * 0.028f); }
            if (x > 500 && x <= 1000) { eff = eff + 11.225f + (x * 0.01857f); }
            if (x > 1000) { eff = eff + 28.795f + (x * 0.001f); }
            return eff;
        }
        public static float Mineexp(float x)
        {
            float exp = 50;
            if (x > 1 && x <= 5) { exp = exp + (x * 10); }
            if (x > 5 && x <= 10) { exp = exp + (x * 15); }
            if (x > 10 && x <= 25) { exp = exp + (x * 20); }
            if (x > 25 && x <= 50) { exp = exp + (x * 25); }
            if (x > 50 && x <= 75) { exp = exp + (x * 30); }
            if (x > 75 && x <= 100) { exp = exp + (x * 45); }
            if (x > 100 && x <= 200) { exp = exp + (x * 65); }
            if (x > 200 && x <= 500) { exp = exp + (x * 95); }
            if (x > 500 && x <= 1000) { exp = exp + (x * 135); }
            if (x > 1000 && x <= 10000) { exp = exp + (x * 140); }
            if (x > 10000 && x <= 100000) { exp = exp + (x * 150); }
            return exp;
        }
        public static float Movecoast(float x)
        {
            float coast = 1250 + (x + 250);
            if (x > 10 && x >= 50)
            {
                coast = coast + ((x - 10) + 400);
                if (x > 50 && x >= 70)
                {
                    coast = coast + ((x - 50) + 600);
                    if (x > 70 && x >= 90)
                    {
                        coast = coast + ((x - 70) + 800);
                        if (x > 90 && x >= 120)
                        {
                            coast = coast + ((x - 90) + 1100);
                            if (x > 120 && x >= 150)
                            {
                                coast = coast + ((x - 120) + 1100);
                                if (x > 150)
                                {
                                    coast = coast + ((x - 150) + 1250);
                                }
                            }
                        }
                    }
                }
            }
            return coast;
        }
        public static float Moveexp(float x)
        {
            float exp = 25 + (x * 25);
            if (x > 10 && x >= 50)
            {
                exp = exp + ((x - 10) + 30);
            }
            if (x > 50 && x >= 70)
            {
                exp = exp + ((x - 50) + 35);
            }
            if (x > 70 && x >= 90)
            {
                exp = exp + ((x - 70) + 45);
            }
            if (x > 90 && x >= 120)
            {
                exp = exp + ((x - 90) + 60);
            }
            if (x > 120 && x >= 150)
            {
                exp = exp + ((x - 120) + 70);
            }
            if (x > 150)
            {
                exp = exp + ((x - 150) + 120);
            }
            return exp;
        }
        public static float Moveeff(float x)
        {
            float eff = 2;
            if (x > 1)
            {
                eff = 2f + (x - 1f) * 0.4f;
                if (x > 50)
                {
                    eff = 2f + 19.6f + ((x - 50f) * 0.5f);
                    if (x > 70)
                    {
                        eff = 2f + 19.6f + 10f + (x - 70f) * 0.4f;
                        if (x > 90)
                        {
                            eff = 2f + 19.6f + 10f + 8f + (x - 90f) * 0.25f;
                            if (x > 120)
                            {
                                eff = 2f + 19.6f + 10f + 8f + 7.5f + ((x - 120f) * 0.15f);
                                if (x > 150)
                                {
                                    eff = 2f + 19.6f + 10f + 8f + 7.5f + 4.5f + ((x - 150f) * 0.011f);
                                    if (eff > 75f)
                                    {
                                        eff = 75f;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return eff;
        }
        public static float Greencoast(float x)
        {
            float coast = 100 * x / 2;
            if (x > 5 && x <= 25) { coast = coast * 1.25f * x; }
            if (x > 25 && x <= 40) { coast = coast * 1.5f * x; }
            if (x > 40 && x <= 75) { coast = coast * 2 * x; }
            if (x > 75 && x <= 150) { coast = coast * 3 * x; }
            if (x > 150 && x <= 200) { coast = coast * 4 * x; }
            if (x > 200) { coast = 8000000 + ((x - 200) * 100000); }
            return coast;
        }
        public static float Greenexp(float x)
        {
            float exp = 1 + x;
            if (x == 1) { exp = exp + 2; }
            if (x == 2) { exp = exp + 5; }
            if (x == 3) { exp = exp + 10; }
            if (x == 4) { exp = exp + 25; }
            if (x == 5) { exp = exp + 40; }
            if (x > 5 && x <= 10) { exp = exp * 1.25f + (10 + x * 2); }
            if (x > 10 && x <= 25) { exp = exp * 1.5f + (10 + x * 2.5f); }
            if (x > 25 && x <= 50) { exp = exp * 1.75f + (10 + x * 3f); }
            if (x > 50 && x <= 100) { exp = exp * 2f + (10 + x * 4f); }
            if (x > 100 && x <= 200) { exp = exp * 3f + (10 + x * 6f); }
            if (x > 200 && x <= 250) { exp = exp * 4f + (10 + x * 7f); }
            if (x > 250) { exp = 2764 + ((x - 250) * 100); }
            return exp;
        }
        public static float Greeneff(float x)
        {
            float eff = 1;
            if (x > 1 && x <= 5) { eff = eff + x; }
            if (x > 5 && x <= 10) { eff = eff + x * 0.9f; }
            if (x > 10 && x <= 25) { eff = eff + x * 0.7f; }
            if (x > 25 && x <= 50) { eff = eff + x * 0.5f; }
            if (x > 50 && x <= 100) { eff = eff + x * 0.3f; }
            if (x > 100 && x <= 250) { eff = eff + x * 0.2f; }
            if (x > 250 && x <= 500) { eff = eff + x * 0.15f; }
            if (x > 500) { eff = 76 + ((x - 500) * 10); }
            return eff;
        }
        public static float Yellowcoast(float x)
        {
            float coast = 125 * x / 2;
            if (x > 5 && x <= 25) { coast = coast * 1.25f * x; }
            if (x > 25 && x <= 40) { coast = coast * 1.5f * x; }
            if (x > 40 && x <= 75) { coast = coast * 2 * x; }
            if (x > 75 && x <= 150) { coast = coast * 3 * x; }
            if (x > 150 && x <= 200) { coast = coast * 4 * x; }
            if (x > 200) { coast = 10000000; }
            return coast;
        }
        public static float Yelloweffect(float x)
        {
            float eff = 1;
            if (x > 1 && x <= 5) { eff = eff + x; }
            if (x > 5 && x <= 10) { eff = eff + x * 0.9f; }
            if (x > 10 && x <= 25) { eff = eff + x * 0.8f; }
            if (x > 25 && x <= 50) { eff = eff + x * 0.7f; }
            if (x > 50 && x <= 100) { eff = eff + x * 0.6f; }
            if (x > 100 && x <= 250) { eff = eff + x * 0.4f; }
            if (x > 250 && x <= 500) { eff = eff + x * 0.3f; }
            if (x > 500) { eff = 151; }
            return eff;
        }
        public static float Yellowexp(float x)
        {
            float exp = 1 + x * 5f;
            if (x == 1) { exp = exp + 2; }
            if (x == 2) { exp = exp + 5; }
            if (x == 3) { exp = exp + 10; }
            if (x == 4) { exp = exp + 25; }
            if (x == 5) { exp = exp + 40; }
            if (x > 5 && x <= 10) { exp = exp * 1.25f + (10 + x * 2); }
            if (x > 10 && x <= 25) { exp = exp * 1.5f + (10 + x * 2.5f); }
            if (x > 25 && x <= 50) { exp = exp * 1.75f + (10 + x * 3f); }
            if (x > 50 && x <= 100) { exp = exp * 2f + (10 + x * 4f); }
            if (x > 100 && x <= 200) { exp = exp * 3f + (10 + x * 6f); }
            if (x > 200 && x <= 250) { exp = exp * 4f + (10 + x * 7f); }
            if (x > 250) { exp = 6764; }
            return exp;
        }
        public static float Redcoast(float x)
        {
            float coast = 175 * x / 2;
            if (x > 5 && x <= 25) { coast = coast * 1.25f * x; }
            if (x > 25 && x <= 40) { coast = coast * 1.5f * x; }
            if (x > 40 && x <= 75) { coast = coast * 2 * x; }
            if (x > 75 && x <= 150) { coast = coast * 3 * x; }
            if (x > 150 && x <= 200) { coast = coast * 4 * x; }
            if (x > 200) { coast = 15000000; }
            return coast;
        }
        public static float Redeffect(float x)
        {
            float eff = 1;
            if (x > 1 && x <= 5) { eff = eff + x; }
            if (x > 5 && x <= 10) { eff = eff + x * 0.9f; }
            if (x > 10 && x <= 25) { eff = eff + x * 0.8f; }
            if (x > 25 && x <= 50) { eff = eff + x * 0.7f; }
            if (x > 50 && x <= 100) { eff = eff + x * 0.6f; }
            if (x > 100 && x <= 250) { eff = eff + x * 0.5f; }
            if (x > 250 && x <= 500) { eff = eff + x * 0.5f; }
            if (x > 500) { eff = 251; }
            return eff;
        }
        public static float Redexp(float x)
        {
            float exp = 1 + x * 8f;
            if (x == 1) { exp = exp + 2; }
            if (x == 2) { exp = exp + 5; }
            if (x == 3) { exp = exp + 10; }
            if (x == 4) { exp = exp + 25; }
            if (x == 5) { exp = exp + 40; }
            if (x > 5 && x <= 10) { exp = exp * 1.25f + (10 + x * 2); }
            if (x > 10 && x <= 25) { exp = exp * 1.5f + (10 + x * 2.5f); }
            if (x > 25 && x <= 50) { exp = exp * 1.75f + (10 + x * 3f); }
            if (x > 50 && x <= 100) { exp = exp * 2f + (10 + x * 4f); }
            if (x > 100 && x <= 200) { exp = exp * 3f + (10 + x * 6f); }
            if (x > 200 && x <= 250) { exp = exp * 4f + (10 + x * 7f); }
            if (x > 250) { exp = 10000; }
            return exp;
        }
        public static float Oporacoast(float x)
        {
            float coast = 250;
            if (x > 1 && x <= 25) { coast = coast + 250 * x; }
            if (x > 25 && x <= 50) { coast = coast + 750 * x; }
            if (x > 50 && x <= 100) { coast = coast + 1250 * x; }
            if (x > 100 && x <= 250) { coast = coast + 2500 * x; }
            if (x > 250) { coast = 625250; }
            return coast;
        }
        public static float Oporaeffect(float x)
        {
            float eff = 1;
            if (x > 1 && x <= 50) { eff = eff - 0.005f * x; }
            if (x > 50 && x <= 172) { eff = eff - 0.0055f * x; }
            if (x > 172) { eff = 0.05f; }
            return eff;
        }
        public static float Oporaexp(float x)
        {
            float exp = 10;
            if (x > 1 && x <= 25) { exp = exp + 2 * x; }
            if (x > 25 && x <= 50) { exp = exp + 5 * x; }
            if (x > 50 && x <= 100) { exp = exp + 5 * x; }
            if (x > 100 && x <= 250) { exp = exp + 20 * x; }
            if (x > 250 && x <= 500) { exp = exp + 25 * x; }
            if (x > 500 && x <= 1000) { exp = exp + 40 * x; }
            if (x > 1000 && x <= 2500) { exp = exp + 50 * x; }
            if (x > 2500) { exp = 125000; }
            return exp;
        }
        public static float Zopcoast(float x)
        {
            float coast = 250;
            if (x > 2 && x <= 10) { coast = coast + x * 2500; }
            if (x > 10 && x <= 50) { coast = (coast + x * 5000) - 250; }
            if (x > 50 && x <= 150) { coast = (coast + x * 7000) - 250; }
            if (x > 150 && x <= 400) { coast = (coast + x * 8000) - 250; }
            if (x > 150 && x <= 400) { coast = (coast + x * 8000) - 250; }
            if (x > 400 && x <= 521) { coast = (coast + x * 9000) - 250; }
            if (x > 521 && x <= 1000) { coast = (coast + x * 12000) - 250; }
            if (x > 1000) { coast = 12500000; }
            return coast;
        }
        public static float Zopexp(float x)
        {
            float exp = 50;
            if (x > 2 && x <= 10) { exp = exp + x * 25; }
            if (x > 10 && x <= 50) { exp = (exp + x * 35) - 50; }
            if (x > 50 && x <= 150) { exp = (exp + x * 45) - 50; }
            if (x > 150 && x <= 250) { exp = (exp + x * 75) - 50; }
            if (x > 250 && x <= 521) { exp = (exp + x * 90) - 50; }
            if (x > 521) { exp = 50000; }
            return exp;
        }
        public static float Zopeffect(float x)
        {
            float eff = 1;
            if (x > 2 && x <= 100) { eff = eff + x * 0.25f; }
            if (x > 100 && x <= 521) { eff = eff + x * 0.174f; }
            if (x > 521) { eff = 92; }
            return eff;
        }
        public static float Roadcoast(float x)
        {
            float coast = 50;
            if (x > 1 && x <= 5) { coast = coast + x * 25; }
            if (x > 5 && x <= 10) { coast = coast + x * 50; }
            if (x > 10 && x <= 25) { coast = coast + x * 75; }
            if (x > 25 && x <= 50) { coast = coast + x * 150; }
            if (x > 50 && x <= 100) { coast = coast + x * 250; }
            if (x > 100 && x <= 500) { coast = coast + x * 1000; }
            if (x > 500 && x <= 1000) { coast = coast + x * 5000; }
            if (x > 1000) { coast = 5250000; }
            return coast;
        }
        public static float Roadexp(float x)
        {
            float exp = 5;
            if (x > 1 & x <= 5) { exp = exp + x * 3; }
            if (x > 5 & x <= 10) { exp = exp + x * 5; }
            if (x > 10 & x <= 50) { exp = exp + x * 7; }
            if (x > 50 & x <= 100) { exp = exp + x * 13; }
            if (x > 100 & x <= 500) { exp = exp + x * 25; }
            if (x > 500 & x <= 1000000) { exp = exp + x * 75; }
            return exp;
        }
        public static float Roadeffect(float x)
        {
            float eff = 5;
            if (x > 1 & x <= 5) { eff = eff - x * 0.1f; }
            if (x > 5 & x <= 51) { eff = eff - 0.15f - x * 0.07f; }
            if (x > 51 & x <= 75) { eff = eff - 1.13f - x * 0.05f; }
            if (x > 75) { eff = 0.1f; }
            return eff;
        }
        public static float Depthcoast(float x)
        {
            float coast = 200 * x / 2;
            if (x > 2 && x <= 5) { coast = coast * x * 3; }
            if (x > 5 && x <= 25) { coast = coast * x * 4; }
            if (x > 25 && x <= 50) { coast = coast * x * 5; }
            if (x > 50 && x <= 75) { coast = coast * x * 7; }
            if (x > 75) { coast = 3937500; }
            return coast;
        }
        public static float Depthexp(float x)
        {
            float exp = 4;
            if (x > 1 && x <= 5) { exp = exp * x * 1.25f; }
            if (x > 5 && x <= 10) { exp = exp * x * 2f; }
            if (x > 10 && x <= 25) { exp = exp * x * 4f; }
            if (x > 25 && x <= 75) { exp = exp * x * 13.33f; }
            if (x > 75 && x <= 125) { exp = exp * x * 15f; }
            if (x > 125 && x <= 500) { exp = exp * x * 15f; }
            return exp;
        }
        public static float Deptheffect(float x)
        {
            float eff = 100;
            if (x > 0) { eff = eff * x; }
            return eff;
        }
        public static float Capacitycoast(float x)
        {
            float coast = 100;
            if (x > 1 && x <= 5) { coast = coast + x * 50; }
            if (x > 5 && x <= 10) { coast = coast + x * 75; }
            if (x > 10 && x <= 25) { coast = coast + x * 150; }
            if (x > 25 && x <= 50) { coast = coast + x * 250; }
            if (x > 50 && x <= 100) { coast = coast + x * 350; }
            if (x > 100 && x <= 500) { coast = coast + x * 500; }
            if (x > 500) { coast = 255000; }
            return coast;
        }
        public static float Capacityexp(float x)
        {
            float exp = 10;
            if (x > 1 && x <= 5) { exp = exp + x * 4; }
            if (x > 5 && x <= 10) { exp = exp + x * 6; }
            if (x > 10 && x <= 25) { exp = exp + x * 7; }
            if (x > 25 && x <= 50) { exp = exp + x * 10; }
            if (x > 50 && x <= 250) { exp = exp + x * 20; }
            if (x > 250 && x <= 500) { exp = exp + x * 25; }
            if (x > 500 && x <= 1000) { exp = exp + x * 35; }
            if (x > 1000) { exp = 40000; }
            return exp;
        }
        public static float Capacityeffect(float x)
        {
            float eff = 50;
            if (x > 0) { eff = eff * x; }
            return eff;
        }
        public static float Compressioncoast(float x)
        {
            float coast = 250;
            if (x > 1 && x <= 5) { coast = coast + x * 75; }
            if (x > 5 && x <= 10) { coast = coast + x * 100; }
            if (x > 10 && x <= 25) { coast = coast + x * 250; }
            if (x > 25 && x <= 50) { coast = coast + x * 500; }
            if (x > 50 && x <= 100) { coast = coast + x * 750; }
            if (x > 100 && x <= 500) { coast = coast + x * 1000; }
            if (x > 500) { coast = 505505; }
            return coast;
        }
        public static float Compressionyexp(float x)
        {
            float exp = 20;
            if (x > 1 && x <= 5) { exp = exp + x * 6; }
            if (x > 5 && x <= 10) { exp = exp + x * 8; }
            if (x > 10 && x <= 25) { exp = exp + x * 10; }
            if (x > 25 && x <= 50) { exp = exp + x * 12; }
            if (x > 50 && x <= 250) { exp = exp + x * 30; }
            if (x > 250 && x <= 500) { exp = exp + x * 35; }
            if (x > 500 && x <= 1000) { exp = exp + x * 50; }
            if (x > 1000) { exp = 50000; }
            return exp;
        }
        public static float Compressioneffect(float x)
        {
            float eff = 100;
            if (x > 0) { eff = 100 * x; }
            return eff;
        }
        public static float Hсompressioncoast(float x)
        {
            float coast = 500;
            if (x > 1 && x <= 5) { coast = coast + x * 250; }
            if (x > 5 && x <= 10) { coast = coast + x * 500; }
            if (x > 10 && x <= 25) { coast = coast + x * 1000; }
            if (x > 25 && x <= 50) { coast = coast + x * 1500; }
            if (x > 50 && x <= 100) { coast = coast + x * 2000; }
            if (x > 100 && x <= 500) { coast = coast + x * 4000; }
            if (x > 500) { coast = 2000000; }
            return coast;
        }
        public static float Hcompressionyexp(float x)
        {
            float exp = 50;
            if (x > 1 && x <= 5) { exp = exp + x * 15; }
            if (x > 5 && x <= 10) { exp = exp + x * 25; }
            if (x > 10 && x <= 25) { exp = exp + x * 40; }
            if (x > 25 && x <= 50) { exp = exp + x * 60; }
            if (x > 50 && x <= 250) { exp = exp + x * 75; }
            if (x > 250 && x <= 500) { exp = exp + x * 100; }
            if (x > 500 && x <= 1000) { exp = exp + x * 150; }
            if (x > 1000) { exp = 150150; }
            return exp;
        }
        public static float Hcompressioneffect(float x)
        {
            float eff = 250;
            if (x > 0) { eff = 250 * x; }
            return eff;
        }
        public static float Nanocoast(float x)
        {
            float coast = 2500;
            if (x > 1 && x <= 5) { coast = coast + x * 1500; }
            if (x > 5 && x <= 10) { coast = coast + x * 2500; }
            if (x > 10 && x <= 25) { coast = coast + x * 5000; }
            if (x > 25 && x <= 50) { coast = coast + x * 10000; }
            if (x > 50 && x <= 100) { coast = coast + x * 20000; }
            if (x > 100 && x <= 500) { coast = coast + x * 30000; }
            if (x > 500) { coast = 15000000; }
            return coast;
        }
        public static float Nanoexp(float x)
        {
            float exp = 250;
            if (x > 1 && x <= 5) { exp = exp + x * 100; }
            if (x > 5 && x <= 10) { exp = exp + x * 125; }
            if (x > 10 && x <= 25) { exp = exp + x * 150; }
            if (x > 25 && x <= 50) { exp = exp + x * 175; }
            if (x > 50 && x <= 250) { exp = exp + x * 200; }
            if (x > 250) { exp = 55555; }

            return exp;
        }
        public static float Nanoeffect(float x)
        {
            float eff = 500;
            if (x > 0) { eff = 500 * x; }
            return eff;
        }
    }
}
