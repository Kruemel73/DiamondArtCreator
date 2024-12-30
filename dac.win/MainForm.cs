using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Serialization;
using Ravlyk.SAE5.WinForms;
using Ravlyk.SAE5.WinForms.Dialogs;
using Ravlyk.SAE5.WinForms.Properties;
using Ravlyk.SAE5.WinForms.UserControls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SAE5.Win
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
        }

        internal static string StartupParameter { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            if (this.CanFocus) { this.Focus(); }
            base.OnLoad(e);
            BeginInvoke(new MethodInvoker(Initialize));
        }

        void Initialize()
        {
            MinimumSize = new Size(768, 540);

            Text = AppInfo.AppDescription;
            if (!string.IsNullOrEmpty(StartupParameter))
            {
                homeUserControl_OpenButtonClicked(null, new HomeUserControl.OpenFileEventArgs(StartupParameter));
            }
            else
            {
                ShowHomeUserControl();
            }

            CheckForUpdates();
        }

        #region Updates

        void CheckForUpdates()
        {
            if (Settings.Default.CheckForUpdatesAtStartup)
            {
                AppInfo.CheckForUpdates(UpdateAvailable, null);
            }
        }

        void UpdateAvailable(string details)
        {
            BeginInvoke(new Action<string>(UpdateAvailableCore), details);
        }

        void UpdateAvailableCore(string details)
        {
            if (string.IsNullOrEmpty(details))
            {
                return;
            }

            UpdateDetails = details;
            homeUserControl?.SetUpdatesLink();

            System.Media.SystemSounds.Beep.Play();
            ShowUpdateDialog(details, this);
        }

        internal string UpdateDetails { get; private set; }

        internal static void ShowUpdateDialog(string details, IWin32Window parent)
        {
            using (var updateDialog = new UpdateDialog(details))
            {
                updateDialog.ShowDialog(parent);
            }
        }

        #endregion

        #region Home

        HomeUserControl homeUserControl;

        void ShowHomeUserControl()
        {
            if (homeUserControl == null)
            {
                homeUserControl = new HomeUserControl(this) { Dock = DockStyle.Fill };
                homeUserControl.NewButtonClicked += homeUserControl_NewButtonClicked;
                homeUserControl.OpenButtonClicked += homeUserControl_OpenButtonClicked;
                homeUserControl.Disposed += HomeUserControl_Disposed;
                Controls.Add(homeUserControl);
                this.Text = "Diamond Art Creator 6 - Startseite";
            }
            homeUserControl.Visible = true;
            closeableControl = null;
        }

        void HomeUserControl_Disposed(object sender, EventArgs e)
        {
            homeUserControl.NewButtonClicked -= homeUserControl_NewButtonClicked;
            homeUserControl.OpenButtonClicked -= homeUserControl_OpenButtonClicked;
            homeUserControl.Disposed -= HomeUserControl_Disposed;
            homeUserControl = null;

            if (Controls.Count == 0)
            {
                ShowHomeUserControl();
            }
        }

        #endregion

        #region Wizard

        void homeUserControl_NewButtonClicked(object sender, EventArgs e)
        {
            ShowWizardUserControl(null);
        }

        void ShowWizardUserControl(CodedImage initialImage)
        {
            returnToImage = initialImage;

            var wizardUserControl = new WizardUserControl(initialImage) { Dock = DockStyle.Fill };
            wizardUserControl.Finished += WizardUserControl_Finished;
            wizardUserControl.Cancelled += WizardUserControl_Cancelled;
            Controls.Add(wizardUserControl);
            closeableControl = wizardUserControl;
            this.Text = "Diamond Art Creator 6 - Assistent";

            homeUserControl?.Dispose();
        }

        CodedImage returnToImage;

        void WizardUserControl_Cancelled(object sender, EventArgs e)
        {
            var wizardUserControl = sender as WizardUserControl;
            if (wizardUserControl != null)
            {
                DisposeWizardUserControl(wizardUserControl);
            }

            if (returnToImage != null)
            {
                ShowSchemeUserControl(returnToImage);
            }
            else
            {
                ShowHomeUserControl();
            }
        }

        void WizardUserControl_Finished(object sender, EventArgs e)
        {
            var wizardUserControl = sender as WizardUserControl;
            if (wizardUserControl != null)
            {
                wizardUserControl.Wizard.FinalImage.HasChanges = true;
                DisposeWizardUserControl(wizardUserControl);
                ShowSchemeUserControl(wizardUserControl.Wizard.FinalImage);
            }
        }

        void DisposeWizardUserControl(WizardUserControl wizardUserControl)
        {
            wizardUserControl.Finished -= WizardUserControl_Finished;
            wizardUserControl.Cancelled -= WizardUserControl_Cancelled;
            wizardUserControl.Dispose();
            closeableControl = null;
        }

        #endregion

        #region Scheme
        void homeUserControl_OpenButtonClicked(object sender, HomeUserControl.OpenFileEventArgs e)
        {
            try
            {
                CodedImage image;
                using (var stream = new FileStream(e.FileName, FileMode.Open, FileAccess.Read))
                {
                    image = ImageSerializer.LoadFromStream(stream, e.FileName);
                    image.FileName = e.FileName;
                    Settings.Default.AddLastOpenFile(e.FileName);
                }

                ShowSchemeUserControl(image);
                homeUserControl?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.ErrorCannotOpenFile + Environment.NewLine + ex.Message);
            }
        }

        void ShowSchemeUserControl(CodedImage image)
        {
            var schemeUserControl = new SchemeUserControl(image) { Dock = DockStyle.Fill };
            schemeUserControl.Disposed += SchemeUserControl_Disposed;

            Controls.Add(schemeUserControl);
            closeableControl = schemeUserControl;
            this.Text = "Diamond Art Creator 6 - Editor";
        }

        void SchemeUserControl_Disposed(object sender, EventArgs e)
        {
            ControlUtilities.UnsubscribeDisposed(sender, SchemeUserControl_Disposed);

            var schemeUserControl = sender as SchemeUserControl;
            if (schemeUserControl?.SchemeImage != null && schemeUserControl.RequstsReturningToWizard)
            {
                var clonedImage = schemeUserControl.SchemeImage.Clone(true);
                clonedImage.FileName = schemeUserControl.SchemeImage.FileName;
                ShowWizardUserControl(clonedImage);
            }
            else
            {
                ShowHomeUserControl();
            }
        }

        #endregion

        #region Close

        ICanClose closeableControl;

        protected override void OnClosing(CancelEventArgs e)
        {
            closeableControl?.OnClosing(e);
            if (!e.Cancel)
            {
                base.OnClosing(e);
            }

            if (!e.Cancel)
            {
                Settings.Default.ManiFormState = WindowState;
                Settings.Default.Save();
            }
        }

        #endregion
    }
}
