using System;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏Drums連打キャラ : CActivity
	{
		// コンストラクタ

		public CAct演奏Drums連打キャラ()
		{
			base.b活性化してない = true;
		}
		
		
		// メソッド
        public virtual void Start( int player )
		{
            for (int i = 0; i < 128; i++)
            {
                if(!RollCharas[i].IsUsing)
                {
                    RollCharas[i].IsUsing = true;
                    RollCharas[i].Type = random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Ptn);
                    RollCharas[i].OldValue = 0;
                    RollCharas[i].Counter = new CCounter(0, 5000, 1, TJAPlayer3.Timer);
                    if (TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
                    {
                        switch (player)
                        {
                            case 0:
                                RollCharas[i].X = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_1P_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_1P_X.Length)];
                                RollCharas[i].Y = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_1P_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_1P_Y.Length)];
                                RollCharas[i].XAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_1P_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_1P_X.Length)];
                                RollCharas[i].YAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_1P_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_1P_Y.Length)];
                                break;
                            case 1:
                                RollCharas[i].X = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_2P_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_2P_X.Length)];
                                RollCharas[i].Y = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_2P_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_2P_Y.Length)];
                                RollCharas[i].XAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_2P_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_2P_X.Length)];
                                RollCharas[i].YAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_2P_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_2P_Y.Length)];
                                break;
                            default:
                                return;
                        }
                    }
                    else
                    {
                        RollCharas[i].X = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_X.Length)];
                        RollCharas[i].Y = TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_StartPoint_Y.Length)];
                        RollCharas[i].XAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_X[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_X.Length)];
                        RollCharas[i].YAdd = TJAPlayer3.Skin.Game_Effect_Roll_Speed_Y[random.Next(0, TJAPlayer3.Skin.Game_Effect_Roll_Speed_Y.Length)];
                    }
                    break;
                }
            }

		}

		// CActivity 実装

		public override void On活性化()
		{
            for (int i = 0; i < 128; i++)
            {
                RollCharas[i] = new RollChara();
                RollCharas[i].IsUsing = false;
                RollCharas[i].Counter = new CCounter();
            }
            // SkinConfigで指定されたいくつかの変数からこのクラスに合ったものに変換していく

            base.On活性化();
		}
		public override void On非活性化()
		{
            for (int i = 0; i < 128; i++)
            {
                RollCharas[i].Counter = null;
            }
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				base.OnManagedリソースの解放();
			}
		}

        public override int On進行描画()
        {
            if (base.b活性化してない)
            {
                return 0;
            }

            for (int i = 0; i < 128; i++)
            {
                var rollChara = RollCharas[i];

                if (!rollChara.IsUsing)
                {
                    continue;
                }

                rollChara.OldValue = rollChara.Counter.n現在の値;
                rollChara.Counter.t進行();
                if (rollChara.Counter.b終了値に達した)
                {
                    rollChara.Counter.t停止();
                    rollChara.IsUsing = false;
                }

                for (int l = rollChara.OldValue; l < rollChara.Counter.n現在の値; l++)
                {
                    rollChara.X += rollChara.XAdd;
                    rollChara.Y += rollChara.YAdd;
                }

                var txRollCharaEffect = TJAPlayer3.Tx.Effects_Roll[rollChara.Type];

                if (txRollCharaEffect == null)
                {
                    continue;
                }

                txRollCharaEffect.t2D描画(TJAPlayer3.app.Device, rollChara.X, rollChara.Y);
                // 画面外にいたら描画をやめさせる
                if (rollChara.X < 0 - txRollCharaEffect.szテクスチャサイズ.Width || rollChara.X > 1280)
                {
                    rollChara.Counter.t停止();
                    rollChara.IsUsing = false;
                }

                if (rollChara.Y < 0 - txRollCharaEffect.szテクスチャサイズ.Height || rollChara.Y > 720)
                {
                    rollChara.Counter.t停止();
                    rollChara.IsUsing = false;
                }
            }

            return 0;
        }


        // その他

		#region [ private ]
		//-----------------
        //private CTexture[] txChara;
        private int nTex枚数;

        [StructLayout(LayoutKind.Sequential)]
        private struct ST連打キャラ
        {
            public int nColor;
            public bool b使用中;
            public CCounter ct進行;
            public int n前回のValue;
            public float fX;
            public float fY;
            public float fX開始点;
            public float fY開始点;
            public float f進行方向; //進行方向 0:左→右 1:左下→右上 2:右→左
            public float fX加速度;
            public float fY加速度;
        }
        private ST連打キャラ[] st連打キャラ = new ST連打キャラ[64];

        [StructLayout(LayoutKind.Sequential)]
        private struct RollChara
        {
            public CCounter Counter;
            public int Type;
            public bool IsUsing;
            public float X;
            public float Y;
            public float XAdd;
            public float YAdd;
            public int OldValue;
        }

        private RollChara[] RollCharas = new RollChara[128];

        private Random random = new Random();

        private int[,] StartPoint;
        private int[,] StartPoint_1P;
        private int[,] StartPoint_2P;
        private float[,] Speed;
        private float[,] Speed_1P;
        private float[,] Speed_2P;
        private int CharaPtn;
        //-----------------
        #endregion
    }
}
