using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.system.screenManager.data {
    public struct dtScreenInfo {
        public static dtScreenInfo Default = new dtScreenInfo();

        public Type Type;
        public string ScreenName;
        public object[] Parameters;
    }
}
