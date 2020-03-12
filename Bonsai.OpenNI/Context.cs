using System;

namespace Bonsai.OpenNI
{
    class Context
    {
        static readonly Context instance = new Context();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Context()
        {
        }

        Context()
        {
            _ = OpenNIWrapper.OpenNI.Initialize();
        }

        ~Context()
        {
            OpenNIWrapper.OpenNI.Shutdown();
        } 

        public static Context Instance => instance;
    }
}
