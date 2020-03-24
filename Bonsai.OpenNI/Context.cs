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
            var status = OpenNIWrapper.OpenNI.Initialize();
            if (status != OpenNIWrapper.OpenNI.Status.Ok)
                throw new Exception("Failed to initialize OpenNI.");
        }

        ~Context()
        {
            OpenNIWrapper.OpenNI.Shutdown();
        } 

        public static Context Instance => instance;
    }
}
