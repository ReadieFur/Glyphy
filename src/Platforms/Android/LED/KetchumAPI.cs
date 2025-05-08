using Android.App;
using Android.Content;
using Com.Nothing.Ketchum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ketchum = Com.Nothing.Ketchum;

namespace Glyphy.Platforms.Android.LED
{
    internal class KetchumAPI : Java.Lang.Object, Ketchum.GlyphManager.ICallback
    {
        private static KetchumAPI? _instance;

        //TODO: Syncronise threads.
        public static async Task<KetchumAPI> GetInstance()
        {
            if (_instance is not null)
                return _instance;

            KetchumAPI instance = new();
            await instance.Init();

            return _instance = instance;
        }

        private TaskCompletionSource<bool> _onServiceConnected = new();

        private async Task Init()
        {
            GlyphManager glyphManager = GlyphManager.GetInstance(Application.Context) ?? throw new Exception("Failed to connect to Glyph API.");
            glyphManager.Init(this);
            await _onServiceConnected.Task;
            Debug.WriteLine("Init");
        }

        public void OnServiceConnected(ComponentName? p0)
        {
            _onServiceConnected.SetResult(true);
            Debug.WriteLine("OnServiceConnected");
        }

        public void OnServiceDisconnected(ComponentName? p0)
        {
        }
    }
}
