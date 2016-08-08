using Sesion2_Lab01.com.isil.system.screenManager.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.system.screenManager {
    public class ScreenManager {

        private Screen mCurrentScreen;

        // Variables temporales
        private List<dtScreenInfo> mNextScreenTypes;

        public Screen CurrentScreen { get { return mCurrentScreen; } }

        public ScreenManager() {
            mNextScreenTypes = new List<dtScreenInfo>();
        }

        public void GotoScreen(dtScreenInfo screenType) {
            mNextScreenTypes.Add(screenType);
        }

        public void GotoScreen(Type screenType) {
            dtScreenInfo screenInfo = dtScreenInfo.Default;
            screenInfo.Type = screenType;

            mNextScreenTypes.Add(screenInfo);
        }

        private void InternalGotoScreen(dtScreenInfo screenType) {
            if (mCurrentScreen != null) {
                mCurrentScreen.Destroy();
                mCurrentScreen = null;
            }

            try {
                ConstructorInfo ci = screenType.Type.GetConstructor(Type.EmptyTypes);

                if (ci != null) {
                    mCurrentScreen = (Screen)Activator.CreateInstance(screenType.Type);
                }
                else {
                    mCurrentScreen = (Screen)Activator.CreateInstance(screenType.Type, 
                        screenType.Parameters);
                }
            }
            catch (Exception exception) { }

            if (mCurrentScreen != null) {
                mCurrentScreen.Initialize();

                System.Diagnostics.Debug.WriteLine("ScreenManager::InternalGotoScreen-> Go to Screen: " + 
                    screenType.Type.Name);
            }
        }
        
        public void Update(int dt) {
            for (int i = 0; i < mNextScreenTypes.Count; i++) {
                dtScreenInfo screenInfo = mNextScreenTypes[i];

                if (screenInfo.Type != null) {
                    this.InternalGotoScreen(screenInfo);
                }

                mNextScreenTypes.RemoveAt(i);
                i--;
            }

            if (mCurrentScreen != null) {
                mCurrentScreen.Update(dt);
            }
        }

        public void Draw(int dt) {
            if (mCurrentScreen != null) {
                mCurrentScreen.Draw(dt);
            }
        }
    }
}
