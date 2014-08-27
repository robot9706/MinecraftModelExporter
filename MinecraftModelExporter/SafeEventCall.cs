using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter
{
    public class SafeEventCall
    {
        public static void CallSafe(Delegate e, params object[] param)
        {
            try
            {
                foreach (Delegate d in e.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(param);
                    }
                    else
                    {
                        syncer.BeginInvoke(d, param);
                    }
                }
            }
            catch { }
        }
    }
}
