using ConquerServer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer
{
    public static class SanityHelper
    {
        private static int MAX_JUMP_DISTANCE = 18;
        public static bool IsBeginnerJob(int job)
        {
            Profession profession = (Profession)(job / 10);
            if (profession == Profession.Trojan ||
                profession == Profession.Warrior ||
                profession == Profession.Archer ||
                profession == Profession.Ninja ||
                profession == Profession.Monk ||
                profession == Profession.Pirate ||
                profession == Profession.Taoist)

                return true;
            
            return false;
        }

        public static void ValidateBeginnerJob(int job)
        {
            if (!IsBeginnerJob(job))
                throw new SanityException($"{job} is not a beginner job");
        }

        public static bool IsValidJump(int x, int y, int nx, int ny, out double distance)
        {
            distance = MathHelper.GetDistance(x, y,nx,ny);
            if (distance > MAX_JUMP_DISTANCE) 
                return false; 

            return true;
        }

        public static bool IsValidJump(int x, int y, int nx, int ny)
        {
            double dummy;
            return IsValidJump(x, y, nx, ny, out dummy);
        }

        public static void ValidateJump(int x, int y, int nx, int ny)
        {
            double distance = 0;
            if(!IsValidJump(x, y, nx, ny, out distance))
                throw new SanityException($"{distance} jump exceeds {MAX_JUMP_DISTANCE}, making it invalid");
        }


        public static void Validate(Func<bool> expression, string message = "")
        {
            if (!expression())
                throw new SanityException($"Sanity check has failed. ${message}");
        }
    }

    public class SanityException : Exception
    {
        public SanityException(string message) 
            : base(message) 
        { 
        }
    }
}
