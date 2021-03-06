using System;
using MonoTouch.UIKit;
using BigTed;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Threading.Tasks;

namespace BTProgressHUDDemo
{
	public class MainViewController : UIViewController
	{
		public MainViewController ()
		{

		}

		float progress = -1;
		NSTimer timer;
		UIAlertView alert;

		public override void LoadView ()
		{
			base.LoadView ();
			View.BackgroundColor = UIColor.LightGray;

			MakeButton ("Show Continuous Progress", () =>
			{
				ProgressHUD.Shared.Ring.Color = UIColor.Green;
				ProgressHUD.Shared.ShowContinuousProgress ("Continuous progress...");
				KillAfter (3);
			});



			MakeButton ("Show", () => {
				ProgressHUD.Shared.Show (); 
				KillAfter ();
			});

			MakeButton ("Show with Cancel", () => {
				ProgressHUD.Shared.Show ("Cancel Me", () => {
					ProgressHUD.Shared.ShowErrorWithStatus ("Operation Cancelled!");
				}, "Please Wait"); 
				//KillAfter ();
			});

			MakeButton ("Show inside Alert", () => {
				alert = new UIAlertView ("Oh, Hai", "Press the button to show it.", null, "Cancel", "Show the HUD");

				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0)
						return;
					ProgressHUD.Shared.Show ("this should never go away?");
					KillAfter ();
				};
				alert.Show ();
			});

			MakeButton ("Show Message", () => {
				ProgressHUD.Shared.Show (status: "Processing your image"); 
				KillAfter ();
			});

			MakeButton ("Show Success", () => {
				ProgressHUD.Shared.ShowSuccessWithStatus ("Great success!");
			});

			MakeButton ("Show Fail", () => {
				ProgressHUD.Shared.ShowErrorWithStatus ("Oh, thats bad");
			});

			MakeButton ("Show Fail 5 seconds", () => {
				ProgressHUD.Shared.ShowErrorWithStatus ("Oh, thats bad", timeoutMs: 5000);
			});

			MakeButton ("Toast", () => {
				ProgressHUD.Shared.ShowToast ("Hello from the toast", showToastCentered: false);

			});

			MakeButton ("Progress", () => {
				progress = 0;
				ProgressHUD.Shared.Show ("Hello!", progress);
				if (timer != null)
				{
					timer.Invalidate ();
				}
				timer = NSTimer.CreateRepeatingTimer (0.5f, delegate
				{
					progress += 0.1f;
					if (progress > 1)
					{
						timer.Invalidate ();
						timer = null;
						ProgressHUD.Shared.Dismiss ();
					} else
					{
						ProgressHUD.Shared.Show ("Hello!", progress);
					}


				});
				NSRunLoop.Current.AddTimer (timer, NSRunLoopMode.Common);
			});

			MakeButton ("Dismiss", () => {
				ProgressHUD.Shared.Dismiss (); 
			});

			//From a bug report from Jose
			MakeButton ("Show, Dismiss, remove cancel", () => {
				ShowWaitDismissWithProperCancelButton ();
			});

		}

		async void ShowWaitDismissWithProperCancelButton ()
		{

			ProgressHUD.Shared.Show ("Cancel", delegate()
			{
				Console.WriteLine ("Canceled.");
			}, "Please wait", -1, ProgressHUD.MaskType.Black);

			var result = await BackgroundSleepOperation ();

			ProgressHUD.Shared.Dismiss ();

			ProgressHUD.Shared.ShowSuccessWithStatus ("Done", 2000);

		}

		async Task<bool> BackgroundSleepOperation ()
		{
			return await Task.Run (() => {
				Thread.Sleep (2000);
				return true;
			});
		}

		void KillAfter (float timeout = 1)
		{
			if (timer != null)
			{
				timer.Invalidate ();
			}
			timer = NSTimer.CreateRepeatingTimer (timeout, delegate
			{
				ProgressHUD.Shared.Dismiss ();
				timer.Invalidate ();
				timer = null;
			});
			NSRunLoop.Current.AddTimer (timer, NSRunLoopMode.Common);
		}

		float y = 20;

		void MakeButton (string text, Action del)
		{
			float x = 20;

			var button = new UIButton (UIButtonType.RoundedRect);
			button.Frame = new RectangleF (x, y, 280, 40);
			button.SetTitle (text, UIControlState.Normal);
			button.TouchUpInside += (o,e) => {
				del ();
			};
			View.Add (button);
		
			
			y += 45;

		}
	}
}

