using PdfSharp.Internal;
using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Processor;
using Ravlyk.SAE.Drawing.Serialization;
using Ravlyk.SAE.Resources;
using Ravlyk.SAE5.WinForms.Dialogs;
using Ravlyk.SAE5.WinForms.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
//using TreeksLicensingLibrary2.EasyIntegration;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Rectangle = System.Drawing.Rectangle;
using SAEWizard = Ravlyk.Drawing.WinForms.SAEWizard;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE5.WinForms.UserControls
{
    public partial class WizardUserControl : UserControl, ICanClose
	{
		public string filename;

		public WizardUserControl()
		{
			InitializeComponent();
		}

		internal WizardUserControl(CodedImage initialImage)
			: this()
		{
			this.initialImage = initialImage;
		}

		CodedImage initialImage;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		//public static TLLInterface tlli = new TLLInterface(SAE5.WinForms.Properties.Resources.chunk, "5ss8:,UaAUhzTE?9trSjSynsxDxTRbn");

		public bool isLizenzValid;
		public bool isLizenzDemo;
		public bool isLizenzCommerc;
		public string fileName;

		void Initialize()
		{
			isLizenzValid = Settings.Default.isLizenzValid;
			isLizenzDemo = Settings.Default.isLizenzDemo;
			isLizenzCommerc = Settings.Default.isLizenzCommerc;

			labelWidth.Left = upDownWidth.Bounds.X + 100;
			labelHeight.Left = upDownHeight.Bounds.X + 100;
			splitContainer.SplitterDistance = splitContainer.Width / 2 - 4;
			splitContainer.SplitterWidth = 8;
			splitContainer.Panel1MinSize = 300;
			splitContainer.Panel2MinSize = 300;

			pictureBox1.Visible = false;
			ribbonLeft.CaptionBarVisible = false;
			ribbonLeft.Minimized = false;
			ribbonLeft.Height = 120;

			ribbonRight.CaptionBarVisible = false;
			ribbonRight.Minimized = false;
			ribbonRight.Height = 120;

			ribbonPanelTransform.Enabled = false;
			ribbonPanelCrop.Enabled = false;
			ribbonTabResultImage.Enabled = false;
			ribbonTabAdvanced.Enabled = false;
			ribbonTabSymbols.Enabled = false;
			ribbonTabPreview.Enabled = false;

			if (isLizenzValid && isLizenzCommerc || isLizenzDemo)
            {
				ribbonPanelEditor.Visible = true;
			}
			if (isLizenzValid && Settings.Default.FT7)
            {
                ribbonPanelEditor.Visible = true;
            }

            if (initialImage != null)
			{
				var sourceFileName = initialImage.SourceImageFileName;
				fileName = sourceFileName;
				using (imageBoxSource.Controller?.SuspendUpdateVisualImage())
				{
					if (!string.IsNullOrEmpty(sourceFileName) && File.Exists(sourceFileName))
					{
						Wizard.LoadSourceImageFromFile(sourceFileName);
					}
					else
					{
						Wizard.ImageSetter.SetNewImage(initialImage);
					}

					RestoreImageSettings(initialImage);
					if (ribbonCheckBoxRound.Checked)
					{
						upDownStitchesPerUnit.TextBoxText = "3,60";
						wizard.ImageResizer.StitchesPerUnit = 3.6m;
					}
				}
				EnableControls();
				initialImage = null;
			}
			else
			{
				// if creating a new scheme open the file-dialog automatically
				buttonOpen_Click(this, EventArgs.Empty);
			}
			ribbonLabelUnit.Text = ribbonCheckBoxCM.Text;
			CheckBoxUnitChanged(ribbonLabelUnit.Text);


			if (!isLizenzValid || isLizenzValid)
			{
				ribbonPanel1.Visible = true;
			}
		}
		
		void RestoreImageSettings(CodedImage image)
		{
			Wizard.RestoreImageSettings(image);
			imageBoxSource.Controller?.UpdateParametersAndVisualImage();
		}

		#region Wizard

		public SAEWizard Wizard
		{
			get
			{
				if (wizard == null)
				{

					SAE.Drawing.Properties.SAEWizardSettings.Default.isLizenzCommerc = Settings.Default.isLizenzCommerc;
					SAE.Drawing.Properties.SAEWizardSettings.Default.isLizenzDemo = Settings.Default.isLizenzDemo;
					SAE.Drawing.Properties.SAEWizardSettings.Default.isLizenzValid = Settings.Default.isLizenzValid;

					wizard = new SAEWizard();
					wizard.SetPalettes(SAEResources.GetAllPalettes(Settings.Default.UserPalettesLocationSafe), "DMC");
					wizard.SetSymbolFonts(SAEResources.GetAllFonts());

					wizard.ImageCropper.PropertyChanged += WizardPropertyChanged;
					wizard.ImageResizer.PropertyChanged += WizardPropertyChanged;
					wizard.ImageColorer.PropertyChanged += WizardPropertyChanged;
					wizard.ImageSymboler.PropertyChanged += WizardPropertyChanged;

					checkBoxFixAspect.Checked = wizard.ImageResizer.KeepAspect;
					ribbonCheckBoxSquare.Checked = wizard.ImageResizer.SquareStones;
					ribbonCheckBoxRound.Checked = wizard.ImageResizer.RoundStones;
					if (ribbonCheckBoxSquare.Checked == true)
					{
						ribbonCheckBoxRound.Checked = false;
					}
					labelHeight.Text = wizard.ImageResizer.Unit.ToString();
					labelWidth.Text = wizard.ImageResizer.Unit.ToString();
					upDownStitchesPerUnit.TextBoxText = wizard.ImageResizer.StitchesPerUnit.ToString("0.00");
					InitializeComboBoxKit();

					upDownMaxColors.TextBoxText = wizard.ImageColorer.MaxColorsCount.ToString();

					foreach (var filterType in Enum.GetValues(typeof(ImageResampler.FilterType)))
					{
						comboBoxResizeFilter.DropDownItems.Add(new RibbonButton(filterType.ToString()));
					}
					comboBoxResizeFilter.TextBoxText = wizard.ImageResizer.FilterType.ToString();

					foreach (var colorComparisonType in Enum.GetValues(typeof(ImageColorsController.ColorComparisonTypes)))
					{
						comboBoxColorsSubstitute.DropDownItems.Add(new RibbonButton(colorComparisonType.ToString()));
					}
					comboBoxColorsSubstitute.TextBoxText = wizard.ImageColorer.ColorComparisonType.ToString();

					checkBoxEnsureBlackAndWhite.Checked = wizard.ImageColorer.EnsureBlackAndWhiteColors;
					upDownDither.TextBoxText = wizard.ImageColorer.DitherLevel.ToString();

					foreach (var fontName in wizard.ImageSymboler.AvailableFontsNames.OrderBy(fName => fName))
					{
						comboBoxSymbols.DropDownItems.Add(new RibbonButton(fontName));
					}
					comboBoxSymbols.TextBoxText = wizard.ImageSymboler.SymbolsFontName;

					imageBoxSource.Controller = new VisualZoomCropController(wizard.ImageCropper, new Size(16, 16));
					imageBoxResult.Controller = new VisualZoomController(wizard.ImageColorer.Manipulator, new Size(16, 16));
				}
				return wizard;
			}
		}

		SAEWizard wizard;

		void InitializeComboBoxKit()
		{
			comboBoxKit.DropDownItems.Clear();
			foreach (var paletteName in wizard.ImageColorer.AvailablePalettesNames.OrderBy(pName => pName))
			{
				comboBoxKit.DropDownItems.Add(new RibbonButton(paletteName));
			}
			comboBoxKit.DropDownItems.Add(new RibbonButton(Resources.ManagePalettes));
			comboBoxKit.TextBoxText = wizard.ImageColorer.PaletteName;
		}

		void WizardPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ImageCropController.CropKind):
					buttonRect.Checked = wizard.ImageCropper.CropKind == ImageCropper.CropKind.Rectangle;
					buttonArc.Checked = wizard.ImageCropper.CropKind == ImageCropper.CropKind.Arc;
					break;
				case nameof(ImageSizeController.SchemeWidth):
					upDownWidth.TextBoxText = wizard.ImageResizer.SchemeWidth.ToString(SizeStringFormat);
					break;
				case nameof(ImageSizeController.SchemeHeight):
					upDownHeight.TextBoxText = wizard.ImageResizer.SchemeHeight.ToString(SizeStringFormat);
					break;
				case nameof(ImageSizeController.RoundStones):
					ribbonCheckBoxSquare.Checked = wizard.ImageResizer.RoundStones;
					if (ribbonCheckBoxRound.Checked)
					{
						//upDownStitchesPerUnit.TextBoxText = "3,60";
						//wizard.ImageResizer.StitchesPerUnit = 3.6m;
						upDownStitchesPerUnit.TextBoxText = wizard.ImageResizer.StoneSize.ToString();
					}
					break;
				case nameof(ImageSizeController.SquareStones):
					ribbonCheckBoxRound.Checked = wizard.ImageResizer.SquareStones;
					if (ribbonCheckBoxSquare.Checked)
					{
						upDownStitchesPerUnit.TextBoxText = "4,00";
						wizard.ImageResizer.StitchesPerUnit = 4;
					}
					break;
				case nameof(ImageSizeController.KeepAspect):
					checkBoxFixAspect.Checked = wizard.ImageResizer.KeepAspect;
					break;
				case nameof(ImageSizeController.Unit):
					labelWidth.Text = wizard.ImageResizer.Unit.ToString();
					labelHeight.Text = wizard.ImageResizer.Unit.ToString();
					upDownStitchesPerUnit.TextBoxText = wizard.ImageResizer.StitchesPerUnit.ToString("0.00");

					/*if (ribbonCheckBoxRound.Checked)
					{
						upDownStitchesPerUnit.TextBoxText = "3,60";
						decimal value;
						if (decimal.TryParse(upDownStitchesPerUnit.TextBoxText, out value))
						{
							wizard.ImageResizer.StitchesPerUnit = value;
						}
					}
					if (ribbonCheckBoxSquare.Checked)
					{
						upDownStitchesPerUnit.TextBoxText = "4,00";
						decimal value;
						if (decimal.TryParse(upDownStitchesPerUnit.TextBoxText, out value))
						{
							wizard.ImageResizer.StitchesPerUnit = value;
						}
					}
					*/
					upDownStitchesPerUnit.Visible = wizard.ImageResizer.Unit != ImageSizeController.SizeUnit.Stitch;
					//upDownStitchesPerUnit.Visible = true;
					break;
				case nameof(ImageSizeController.StitchesPerUnit):
					upDownStitchesPerUnit.TextBoxText = wizard.ImageResizer.StitchesPerUnit.ToString("0.00");
					break;
				case nameof(ImageSizeController.FilterType):
					comboBoxResizeFilter.TextBoxText = wizard.ImageResizer.FilterType.ToString();
					break;
				case nameof(ImageColorsController.PaletteName):
					comboBoxKit.TextBoxText = wizard.ImageColorer.PaletteName;
					break;
				case nameof(ImageColorsController.MaxColorsCount):
					upDownMaxColors.TextBoxText = wizard.ImageColorer.MaxColorsCount.ToString();
					break;
				case nameof(ImageColorsController.EnsureBlackAndWhiteColors):
					checkBoxEnsureBlackAndWhite.Checked = wizard.ImageColorer.EnsureBlackAndWhiteColors;
					break;
				case nameof(ImageColorsController.ColorComparisonType):
					comboBoxColorsSubstitute.TextBoxText = wizard.ImageColorer.ColorComparisonType.ToString();
					break;
				case nameof(ImageColorsController.DitherLevel):
					upDownDither.TextBoxText = wizard.ImageColorer.DitherLevel.ToString();
					break;
				case nameof(ImageSymbolsController.SymbolsFontName):
					comboBoxSymbols.TextBoxText = wizard.ImageSymboler.SymbolsFontName;
					break;
			}
		}

		string SizeStringFormat => wizard.ImageResizer.Unit == ImageSizeController.SizeUnit.Stitch ? "0" : "0.00";

		#endregion

		#region Source image controls events handlers

		void buttonOpen_Click(object sender, EventArgs e)
		{
			OpenImage();
		}
		void OpenImage()
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = Resources.FileFilterImages;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					try
					{
						fileName = openFileDialog.FileName;
						using (imageBoxSource.Controller?.SuspendUpdateVisualImage())
						{
							if (Path.GetExtension(fileName)?.Equals(".dac", StringComparison.OrdinalIgnoreCase) ?? false)
							{
								CodedImage image;
								using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
								{
									image = ImageSerializer.LoadFromStream(stream, fileName);
								}
								if (!string.IsNullOrEmpty(image?.SourceImageFileName) && File.Exists(image.SourceImageFileName))
								{
									var choice = MessageBox.Show(
										string.Format(Resources.WizardLoadSourceImageInsteadOfScheme, fileName, image.SourceImageFileName),
										Resources.WizardLoadImage, MessageBoxButtons.YesNoCancel);

									switch (choice)
									{
										case DialogResult.Cancel:
											return;
										case DialogResult.Yes:
											fileName = image.SourceImageFileName;
											RestoreImageSettings(image);
											break;
										case DialogResult.No:
											Wizard.ImageSetter.SetNewImage(image);
											RestoreImageSettings(image);
											EnableControls();
											return;
									}
								}
							}
							Wizard.LoadSourceImageFromFile(fileName);
						}
						EnableControls();
					}
					catch (Exception ex)
					{
						MessageBox.Show(Resources.ErrorCannotOpenFile + Environment.NewLine + ex.Message);
					}
				}
			}
		}

		void EnableControls()
		{
			ribbonPanelTransform.Enabled = true;
			ribbonPanelCrop.Enabled = true;

			ribbonTabResultImage.Enabled = true;
			ribbonTabAdvanced.Enabled = true;
			ribbonTabSymbols.Enabled = true;
			ribbonTabPreview.Enabled = true;
			checkBoxDigits.Checked = true;
			checkBoxLetters.Checked = true;
			checkBoxSymbols.Checked = true;

			wizard.ImageSymboler.IncludeNumbers = checkBoxDigits.Checked;
			wizard.ImageSymboler.IncludeLetters = checkBoxLetters.Checked;
			wizard.ImageSymboler.IncludeSymbols = checkBoxSymbols.Checked;
			buttonNext.Enabled = true;
		}

		void buttonRotateLeft_Click(object sender, EventArgs e)
		{
			Wizard.ImageRotator.RotateCCW();
		}

		void buttonRotateRight_Click(object sender, EventArgs e)
		{
			Wizard.ImageRotator.RotateCW();
		}

		void buttonFlipHorizontally_Click(object sender, EventArgs e)
		{
			Wizard.ImageRotator.FlipHorizontally();
		}

		void buttonFlipVertically_Click(object sender, EventArgs e)
		{
			Wizard.ImageRotator.FlipVertically();
		}

		void buttonCropRect_Click(object sender, EventArgs e)
		{
			buttonRect.Checked = !buttonRect.Checked;
			((VisualZoomCropController)imageBoxSource.Controller).CropKind = buttonRect.Checked ? ImageCropper.CropKind.Rectangle : ImageCropper.CropKind.None;
		}

		void buttonCropCircle_Click(object sender, EventArgs e)
		{
			buttonArc.Checked = !buttonArc.Checked;
			((VisualZoomCropController)imageBoxSource.Controller).CropKind = buttonArc.Checked ? ImageCropper.CropKind.Arc : ImageCropper.CropKind.None;
		}

		#endregion

		#region Preview controls events handlers

		void ribbonRight_ActiveTabChanged(object sender, EventArgs e)
		{
			if (wizard == null && ribbonRight.ActiveTab != ribbonTabResultImage)
			{
				ribbonRight.ActiveTab = ribbonTabResultImage;
				System.Media.SystemSounds.Beep.Play();
				return;
			}

			if (ribbonRight.ActiveTab == ribbonTabResultImage || ribbonRight.ActiveTab == ribbonTabAdvanced)
			{
				tabControlRight.SelectedTab = tabPageResultImage;
			}
			else if (ribbonRight.ActiveTab == ribbonTabSymbols)
			{
				tabControlRight.SelectedTab = tabPageSymbols;
			}
			else if (ribbonRight.ActiveTab == ribbonTabPreview)
			{
				tabControlRight.SelectedTab = tabPagePreview;
			}
		}

		void tabControlRight_Selected(object sender, TabControlEventArgs e)
		{
			if (e.TabPage == tabPageSymbols && scrollControlSymbols.Controller == null)
			{
				scrollControlSymbols.Controller = new VisualSymbolsController(Wizard.ImageSymboler, new Size(scrollControlSymbols.Width, scrollControlSymbols.Height));
				scrollControlSymbols.Controller.VisualImageChanged += Symbols_VisualImageChanged;
			}
			else if (e.TabPage == tabPagePreview && scrollControlGridPreview.Controller == null)
			{
				var patternGridController = new VisualPatternGridController(Wizard.ImageSymboler.Manipulator, Wizard.ImageSymboler.SymbolsFont, SAEResources.GetCrossImage());
				scrollControlGridPreview.Controller = patternGridController;

				switch (patternGridController.PaintMode)
				{
					case PatternGridPainter.StitchesPaintMode.Symbols:
						buttonSymbols.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.WhiteSymbols:
						buttonWhiteSymbols.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.ColoredSymbols:
						buttonColorSymbols.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.HalfTones:
						buttonHalfTone.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.Full:
						buttonFull.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.Colors:
						buttonColors.Checked = true;
						break;
					case PatternGridPainter.StitchesPaintMode.Cross:
						buttonCross.Checked = true;
						break;
				}
				upDownCellSize.TextBoxText = patternGridController.CellSize.ToString();
				checkBoxRulers.Checked = patternGridController.ShowRulers;
				checkBoxLines.Checked = patternGridController.ShowLines;
			}
		}

		VisualPatternGridController PatternGridController => scrollControlGridPreview.Controller as VisualPatternGridController;

		void buttonRandomSymbols_Click(object sender, EventArgs e)
		{
			using (Wizard.ImageSymboler.SuspendCallManipulations())
			{
				Wizard.ImageSymboler.ClearAllSelection();
				Wizard.ImageSymboler.AddRandomSymbols();
			}
		}

		void buttonClearSymbols_Click(object sender, EventArgs e)
		{
			Wizard.ImageSymboler.ClearAllSelection();
		}

		void Symbols_VisualImageChanged(object sender, EventArgs e)
		{
			labelSelectedNo.Text = string.Format(Resources.LabelsSelectedSymbolsCount,
				Wizard.ImageSymboler.SelectedCount, Wizard.ImageSymboler.Manipulator.ManipulatedImage.Palette.Count);
		}

		#endregion

		#region Finish

		void buttonNext_Click(object sender, EventArgs e)
		{
			tabControlRight.Focus();
			if (!ribbonCheckBoxBuegelperlen.Checked)
			{
				wizard.ImageResizer.StickPlateHeight = Wizard.FinalImage.Size.Height;
				wizard.ImageResizer.StickPlateWidth = Wizard.FinalImage.Size.Width;
            }

			if (Wizard.ImageSymboler.SelectedCount < Wizard.ImageSymboler.Manipulator.ManipulatedImage.Palette.Count)
			{
				Wizard.ImageSymboler.AddRandomSymbols();
			}
			Wizard.FinalImage.SourceImageFileName = Wizard.ImageSetter.ImageSourceDescription;
			Wizard.FinalImage.Palette.Name = Wizard.ImageColorer.PaletteName;
			/*if (ribbonCheckBoxRound.Checked == true)
			{
				wizard.ImageResizer.StitchesPerUnit = 3.6m;
			}
			if (ribbonCheckBoxSquare.Checked == true)
			{
				wizard.ImageResizer.StitchesPerUnit = 4m;
			}*/
			Wizard.SaveImageSettings(Wizard.FinalImage);
			Wizard.SaveDefaults();

			Finished?.Invoke(this, EventArgs.Empty);
		}

		void buttonCancel_Click(object sender, EventArgs e)
		{
			Cancelled?.Invoke(this, EventArgs.Empty);
		}

		void ICanClose.OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			buttonCancel_Click(this, EventArgs.Empty);
		}

		public event EventHandler Finished;
		public event EventHandler Cancelled;

		#endregion

		#region Result image controls

		void checkBoxFixAspect_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			if (wizard.ImageResizer.KeepAspect != checkBoxFixAspect.Checked)
			{
				wizard.ImageResizer.KeepAspect = checkBoxFixAspect.Checked;
			}
		}

		void upDownWidth_TextBoxValidated(object sender, EventArgs e)
		{
			decimal value;
			if (decimal.TryParse(upDownWidth.TextBoxText, out value))
			{
				wizard.ImageResizer.SchemeWidth = value;
			}
			upDownWidth.TextBoxText = wizard.ImageResizer.SchemeWidth.ToString(SizeStringFormat);
			ShowCountPlates();
		}

		void upDownWidth_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.SchemeWidth++;
		}

		void upDownWidth_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.SchemeWidth--;
		}

		void upDownHeight_TextBoxValidated(object sender, EventArgs e)
		{
			decimal value;
			if (decimal.TryParse(upDownHeight.TextBoxText, out value))
			{
				wizard.ImageResizer.SchemeHeight = value;
			}
			upDownHeight.TextBoxText = wizard.ImageResizer.SchemeHeight.ToString(SizeStringFormat);
			ShowCountPlates();
		}

		void upDownHeight_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.SchemeHeight++;
		}

		void upDownHeight_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.SchemeHeight--;
		}

		void upDownStitchesPerUnit_TextBoxValidated(object sender, EventArgs e)
		{
			decimal value;
			if (decimal.TryParse(upDownStitchesPerUnit.TextBoxText, out value))
			{
				wizard.ImageResizer.StitchesPerUnit = value;
			}
			upDownStitchesPerUnit.TextBoxText = wizard.ImageResizer.StitchesPerUnit.ToString("0.00");
		}

		void upDownStitchesPerUnit_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.StitchesPerUnit += 0.1m;
		}

		void upDownStitchesPerUnit_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageResizer.StitchesPerUnit -= 0.1m;
		}

		void comboBoxKit_TextBoxTextChanged(object sender, EventArgs e)
		{
			if (inComboBoxKitTextChange)
			{
				return;
			}

			if (comboBoxKit.TextBoxText == Resources.ManagePalettes)
			{
				inComboBoxKitTextChange = true;
				try
				{
					using (var threadsManageDialog = new ThreadsManagementDialog())
					{
						threadsManageDialog.ShowDialog(this);
					}
					wizard.ImageColorer.AddColorPalettes(SAEResources.GetAllPalettes(Settings.Default.UserPalettesLocation), true);
					InitializeComboBoxKit();
				}
				finally
				{
					inComboBoxKitTextChange = false;
				}
			}
			else
			{
				wizard.ImageColorer.PaletteName = comboBoxKit.TextBoxText;
			}
		}
		bool inComboBoxKitTextChange;

		void upDownMaxColors_TextBoxValidated(object sender, EventArgs e)
		{
			int value;
			if (int.TryParse(upDownMaxColors.TextBoxText, out value))
			{
				wizard.ImageColorer.MaxColorsCount = value;
			}
			upDownMaxColors.TextBoxText = wizard.ImageColorer.MaxColorsCount.ToString();
			ribbonLabelCount.Text = Wizard.ImageSymboler.Manipulator.ManipulatedImage.Palette.Count.ToString();
		}

		void upDownMaxColors_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageColorer.MaxColorsCount++;
		}

		void upDownMaxColors_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageColorer.MaxColorsCount--;
		}

		void comboBoxResizeFilter_TextBoxTextChanged(object sender, EventArgs e)
		{
			ImageResampler.FilterType filterType;
			if (Enum.TryParse(comboBoxResizeFilter.TextBoxText, out filterType))
			{
				wizard.ImageResizer.FilterType = filterType;
			}
		}

		void comboBoxColorsSubstitute_TextBoxTextChanged(object sender, EventArgs e)
		{
			ImageColorsController.ColorComparisonTypes colorComparisonType;
			if (Enum.TryParse(comboBoxColorsSubstitute.TextBoxText, out colorComparisonType))
			{
				wizard.ImageColorer.ColorComparisonType = colorComparisonType;
			}
		}

		void checkBoxEnsureBlackAndWhite_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			wizard.ImageColorer.EnsureBlackAndWhiteColors = checkBoxEnsureBlackAndWhite.Checked;
		}

		void upDownDither_TextBoxValidated(object sender, EventArgs e)
		{
			int value;
			if (int.TryParse(upDownDither.TextBoxText, out value))
			{
				wizard.ImageColorer.DitherLevel = value;
			}
			upDownDither.TextBoxText = wizard.ImageColorer.DitherLevel.ToString();
		}

		void upDownDither_UpButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageColorer.DitherLevel++;
		}

		void upDownDither_DownButtonClicked(object sender, MouseEventArgs e)
		{
			wizard.ImageColorer.DitherLevel--;
		}

		void comboBoxSymbols_TextBoxTextChanged(object sender, EventArgs e)
		{
			wizard.ImageSymboler.SymbolsFontName = comboBoxSymbols.TextBoxText;
		}

		void buttonSymbols_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Symbols;
			buttonSymbols.Checked = true;
		}

		void buttonWhiteSymbols_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.WhiteSymbols;
			buttonWhiteSymbols.Checked = true;
		}

		void buttonColorSymbols_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.ColoredSymbols;
			buttonColorSymbols.Checked = true;
		}

		void buttonHalfTone_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.HalfTones;
			buttonHalfTone.Checked = true;
		}

		void buttonFull_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Full;
			buttonFull.Checked = true;
		}

		void buttonColors_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Colors;
			buttonColors.Checked = true;
		}

		void buttonCross_Click(object sender, EventArgs e)
		{
			PatternGridController.PaintMode = PatternGridPainter.StitchesPaintMode.Cross;
			buttonCross.Checked = true;
		}

		void upDownCellSize_UpButtonClicked(object sender, MouseEventArgs e)
		{
			if (!zoomChangingLock.IsLocked())
			{
				using (DisposableLock.Lock(ref zoomChangingLock))
				{
					PatternGridController.CellSize++;
					upDownCellSize.TextBoxText = PatternGridController.CellSize.ToString();
					zoomSliderCellSize.ZoomValue = PatternGridController.CellSize;
				}
			}
		}

		void upDownCellSize_DownButtonClicked(object sender, MouseEventArgs e)
		{
			if (!zoomChangingLock.IsLocked())
			{
				using (DisposableLock.Lock(ref zoomChangingLock))
				{
					PatternGridController.CellSize--;
					upDownCellSize.TextBoxText = PatternGridController.CellSize.ToString();
					zoomSliderCellSize.ZoomValue = PatternGridController.CellSize;
				}
			}
		}
		private void zoomSliderCellSize_ZoomChanged(object sender, EventArgs e)
		{
			if (!zoomChangingLock.IsLocked())
			{
				using (DisposableLock.Lock(ref zoomChangingLock))
				{
					PatternGridController.CellSize = zoomSliderCellSize.ZoomValue;
					zoomSliderCellSize.ZoomValue = PatternGridController.CellSize;
					upDownCellSize.TextBoxText = PatternGridController.CellSize.ToString();
				}
			}
		}

		void checkBoxRulers_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			PatternGridController.ShowRulers = checkBoxRulers.Checked;
		}

		void checkBoxLines_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			PatternGridController.ShowLines = checkBoxLines.Checked;
		}

		DisposableLock zoomChangingLock;

		#endregion

		private void ribbonCheckBoxCM_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			upDownStitchesPerUnit.Visible = true;
			ribbonCheckBoxStones.Checked = false;
			ribbonCheckBoxCM.Checked = true;
			ribbonLabelUnit.Text = ribbonCheckBoxCM.Text;
			CheckBoxUnitChanged(ribbonLabelUnit.Text);
		}

		private void ribbonCheckBoxStones_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			upDownStitchesPerUnit.Visible = false;
			ribbonCheckBoxCM.Checked = false;
			ribbonCheckBoxStones.Checked = true;
			ribbonLabelUnit.Text = "Stitch";
			CheckBoxUnitChanged(ribbonLabelUnit.Text);
		}

		private void CheckBoxUnitChanged(object sender)
		{
			ImageSizeController.SizeUnit value;

			if (wizard != null)
			{
				if (Enum.TryParse(ribbonLabelUnit.Text, out value))
				{
					if (wizard.ImageResizer.Unit != value)
					{
						wizard.ImageResizer.Unit = value;
						labelWidth.Text = wizard.ImageResizer.Unit.ToString();
						labelHeight.Text = wizard.ImageResizer.Unit.ToString();
					}
				}
				var newValue = wizard.ImageResizer.Unit.ToString();
			}
		}

		private void ribbonCheckBoxSquare_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			bool wasChecked = ribbonCheckBoxCM.Checked;
			if (ribbonCheckBoxSquare.Checked)
			{
				//ribbonCheckBoxStones.Checked = false;
				//ribbonCheckBoxCM.Checked = true;
				//ribbonLabelUnit.Text = ribbonCheckBoxCM.Text;
				//CheckBoxUnitChanged(ribbonLabelUnit.Text);

				ribbonLabelUnit.Text = ribbonCheckBoxCM.Text;
				CheckBoxUnitChanged(ribbonLabelUnit.Text);

				ribbonCheckBoxBuegelperlen.Checked = false;
				ribbonPanelStiftplatten.Visible = false;
				ribbonPanelBuegelperlen.Visible = false;

				ribbonCheckBoxRound.Checked = false;
				wizard.ImageResizer.SquareStones = true;
				wizard.ImageResizer.RoundStones = false;
				wizard.ImageResizer.Spu = 4;
				wizard.ImageResizer.StoneSize = 2.5;

			}

			SquareStones(e);
			if (!wasChecked)
			{
				ribbonLabelUnit.Text = "Stitch";
				CheckBoxUnitChanged(ribbonLabelUnit.Text);
			}
		}

		private void ribbonCheckBoxRound_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			bool wasChecked = ribbonCheckBoxCM.Checked;
			if (ribbonCheckBoxRound.Checked)
			{
				//ribbonCheckBoxStones.Checked = false;
				//ribbonCheckBoxCM.Checked = true;
				//ribbonLabelUnit.Text = ribbonCheckBoxCM.Text;
				//CheckBoxUnitChanged(ribbonLabelUnit.Text);

				ribbonLabelUnit.Text = ribbonCheckBoxCM.Text;
				CheckBoxUnitChanged(ribbonLabelUnit.Text);

				ribbonCheckBoxSquare.Checked = false;
				wizard.ImageResizer.RoundStones = true;
				wizard.ImageResizer.SquareStones = false;
				wizard.ImageResizer.Spu = 3.6;
				wizard.ImageResizer.StoneSize = 2.8;
			}

			RoundStones(e);
			if (!wasChecked)
			{
				ribbonLabelUnit.Text = "Stitch";
				CheckBoxUnitChanged(ribbonLabelUnit.Text);
			}
		}

		private void SquareStones(EventArgs e)
		{
			if (ribbonCheckBoxSquare.Checked)
			{
				wizard.ImageResizer.SquareStones = ribbonCheckBoxSquare.Checked;
				wizard.ImageResizer.RoundStones = ribbonCheckBoxRound.Checked;
				//ribbonCheckBoxRound.Checked = true;
				upDownStitchesPerUnit.TextBoxText = wizard.ImageResizer.Spu.ToString();
				wizard.ImageResizer.StitchesPerUnit = (decimal)wizard.ImageResizer.Spu;
				upDownStitchesPerUnit_TextBoxValidated(upDownStitchesPerUnit.TextBoxText, e);
			}
		}

		private void RoundStones(EventArgs e)
		{
			if (ribbonCheckBoxRound.Checked)
			{
				wizard.ImageResizer.SquareStones = ribbonCheckBoxSquare.Checked;
				wizard.ImageResizer.RoundStones = ribbonCheckBoxRound.Checked;
				//ribbonCheckBoxRound.Checked = true;
				//spu = 3.6;
				upDownStitchesPerUnit.TextBoxText = wizard.ImageResizer.Spu.ToString();
				wizard.ImageResizer.StitchesPerUnit = (decimal)wizard.ImageResizer.Spu;
				upDownStitchesPerUnit_TextBoxValidated(upDownStitchesPerUnit.TextBoxText, e);
			}
		}

		private void checkBoxDigits_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			wizard.ImageSymboler.IncludeNumbers = checkBoxDigits.Checked;
		}

		private void checkBoxLetters_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			wizard.ImageSymboler.IncludeLetters = checkBoxLetters.Checked;
		}

		private void checkBoxSymbols_CheckBoxCheckChanged(object sender, EventArgs e)
		{
			wizard.ImageSymboler.IncludeSymbols = checkBoxSymbols.Checked;
		}

		private void ribbonButtonSaveSymbols_Click(object sender, EventArgs e)
		{
			// schreiben in Datei os

			if (!Directory.Exists(Settings.Default.UserPalettesLocation))
			{
				Directory.CreateDirectory(Settings.Default.UserPalettesLocation);
			}
			
				filename = Settings.Default.UserPalettesLocation + @"\os.txt"; 
			
			using (FileStream fs = File.OpenWrite(filename))
			{
				StreamWriter sw = new StreamWriter(fs);
				foreach (var color in wizard.ImageSymboler.SelectedSymbols)
				{
					string exp = color.ToString();
					sw.WriteLine(exp);
				}
				sw.Close();
			}
		}

        private void stiftplattefalse(EventArgs e)
        {

			ribbonCheckBoxMidi14.Checked = false;
			ribbonCheckBoxMidi29.Checked = false;

		}

        private void ribbonCheckBoxArtkalMini_CheckBoxCheckChanged(object sender, EventArgs e)
        {
			if (ribbonCheckBoxArtkalMini.Checked)
			{
				ribbonCheckBoxStones.Checked = false;
				ribbonCheckBoxCM.Checked = true;
				ribbonLabelUnit.Text = ribbonCheckBoxCM.Text;
				CheckBoxUnitChanged(ribbonLabelUnit.Text);
				ribbonCheckBoxSquare.Checked = false;
				ribbonCheckBoxRound.Checked = true;

				ribbonCheckBoxArtkalMidi.Checked = false;

				//Set Plates to Zero
				wizard.ImageResizer.StickPlateHeight = 0;
				wizard.ImageResizer.StickPlateWidth = 0;
				Settings.Default.PlateHight = 0;
				Settings.Default.PlateWidth = 0;
				ribbonCheckBoxArtkalMini27.Checked = false;
				ribbonCheckBoxArtkalMini50.Checked = false;
				ribbonCheckBoxMidi14.Checked = false;
				ribbonCheckBoxMidi29.Checked = false;
				ShowCountPlates();

					ribbonPanelStiftplatten.Visible = true;
					ribbonCheckBoxArtkalMini50.Visible = true;
					ribbonCheckBoxArtkalMini27.Visible = true;
					ribbonCheckBoxMidi14.Visible = false;
					ribbonCheckBoxMidi29.Visible = false;
					ribbonUpDownPlateHeight.Visible = false;
					ribbonUpDownPlateWidth.Visible = false;

					stiftplattefalse(e);

				wizard.ImageResizer.RoundStones = true;
				wizard.ImageResizer.SquareStones = false;
				wizard.ImageResizer.StoneSize = 2.6;
				wizard.ImageResizer.Spu = 3.9;
			}
			RoundStones(e);
		}

        private void ribbonCheckBoxArtkalMidi_CheckBoxCheckChanged(object sender, EventArgs e)
        {
			if (ribbonCheckBoxArtkalMidi.Checked)
			{
				ribbonCheckBoxStones.Checked = false;
				ribbonCheckBoxCM.Checked = true;
				ribbonLabelUnit.Text = ribbonCheckBoxCM.Text;
				CheckBoxUnitChanged(ribbonLabelUnit.Text);
				ribbonCheckBoxSquare.Checked = false;
				ribbonCheckBoxRound.Checked = true;

				ribbonCheckBoxArtkalMini.Checked = false;

				//Set Plates to Zero
				wizard.ImageResizer.StickPlateHeight = 0;
				wizard.ImageResizer.StickPlateWidth = 0;
				ribbonCheckBoxArtkalMini27.Checked = false;
				ribbonCheckBoxArtkalMini50.Checked = false;
				ribbonCheckBoxMidi14.Checked = false;
				ribbonCheckBoxMidi29.Checked = false;
				ShowCountPlates();

					ribbonPanelStiftplatten.Visible = true;
					ribbonCheckBoxArtkalMini50.Visible = false;
					ribbonCheckBoxArtkalMini27.Visible = false;
					ribbonCheckBoxMidi14.Visible = true;
					ribbonCheckBoxMidi29.Visible = true;
					ribbonUpDownPlateHeight.Visible = false;
					ribbonUpDownPlateWidth.Visible = false;

					stiftplattefalse(e);

				wizard.ImageResizer.RoundStones = true;
				wizard.ImageResizer.SquareStones = false;
				wizard.ImageResizer.StoneSize = 5;
				wizard.ImageResizer.Spu = 2;
			}
			RoundStones(e);
		}
        private void ribbonCheckBoxArtkalMini50_CheckBoxCheckChanged(object sender, EventArgs e)
        {
			wizard.ImageResizer.StickPlateHeight = 50;
			wizard.ImageResizer.StickPlateWidth = 50;
			Settings.Default.PlateHight = 50;
			Settings.Default.PlateWidth = 50;
			ribbonUpDownPlateHeight.TextBoxText = wizard.ImageResizer.StickPlateHeight.ToString();
			ribbonUpDownPlateWidth.TextBoxText = wizard.ImageResizer.StickPlateWidth.ToString();

			ribbonCheckBoxArtkalMini27.Checked = false;
			ribbonCheckBoxMidi14.Checked = false;
			ribbonCheckBoxMidi29.Checked = false;
			if (ribbonCheckBoxArtkalMini50.Checked == false)
			{
				wizard.ImageResizer.StickPlateHeight = 0;
				wizard.ImageResizer.StickPlateWidth = 0;
			}
			ShowCountPlates();
		}

        private void ShowCountPlates()
        {
			double plateCountWidth = wizard.ImageResizer.Width / wizard.ImageResizer.StickPlateWidth;
			double plateCountHeight = wizard.ImageResizer.Height / wizard.ImageResizer.StickPlateHeight;
			var countPlateW = Math.Ceiling(plateCountWidth);
			var countPlageH = Math.Ceiling(plateCountHeight);
			var countPlatesUsed = Math.Ceiling(countPlageH * countPlateW);
			ribbonLabelUsedPlates.Text = countPlateW.ToString() + " x " + countPlageH.ToString() + " = " + countPlatesUsed.ToString() + " Plates";
		}

        private void ribbonCheckBoxArtkalMini27_CheckBoxCheckChanged(object sender, EventArgs e)
		{
				wizard.ImageResizer.StickPlateHeight = 27;
				wizard.ImageResizer.StickPlateWidth = 27;
				Settings.Default.PlateHight = 27;
				Settings.Default.PlateWidth = 27;
			ribbonUpDownPlateHeight.TextBoxText = wizard.ImageResizer.StickPlateHeight.ToString();
				ribbonUpDownPlateWidth.TextBoxText = wizard.ImageResizer.StickPlateWidth.ToString();
				

				ribbonCheckBoxMidi14.Checked = false;
				ribbonCheckBoxMidi29.Checked = false;
				ribbonCheckBoxArtkalMini50.Checked = false;
			if (ribbonCheckBoxArtkalMini27.Checked == false)
			{
				wizard.ImageResizer.StickPlateHeight = 0;
				wizard.ImageResizer.StickPlateWidth = 0;
			}
			ShowCountPlates();
		}

		private void ribbonCheckBoxMidi14_CheckBoxCheckChanged(object sender, EventArgs e)
        {
			wizard.ImageResizer.StickPlateHeight = 14;
			wizard.ImageResizer.StickPlateWidth = 14;
			Settings.Default.PlateHight = 14;
			Settings.Default.PlateWidth = 14;
			ribbonUpDownPlateHeight.TextBoxText = wizard.ImageResizer.StickPlateHeight.ToString();
			ribbonUpDownPlateWidth.TextBoxText = wizard.ImageResizer.StickPlateWidth.ToString();

			ribbonCheckBoxArtkalMini50.Checked = false;
			ribbonCheckBoxArtkalMini27.Checked = false;
			ribbonCheckBoxMidi29.Checked = false;
			if (ribbonCheckBoxMidi14.Checked == false)
			{
				wizard.ImageResizer.StickPlateHeight = 0;
				wizard.ImageResizer.StickPlateWidth = 0;
			}
			ShowCountPlates();
		}

        private void ribbonCheckBoxMidi29_CheckBoxCheckChanged(object sender, EventArgs e)
        {
			wizard.ImageResizer.StickPlateHeight = 29;
			wizard.ImageResizer.StickPlateWidth = 29;
			Settings.Default.PlateHight = 29;
			Settings.Default.PlateWidth = 29;
			ribbonUpDownPlateHeight.TextBoxText = wizard.ImageResizer.StickPlateHeight.ToString();
			ribbonUpDownPlateWidth.TextBoxText = wizard.ImageResizer.StickPlateWidth.ToString();
			
			ribbonCheckBoxArtkalMini50.Checked = false;
			ribbonCheckBoxArtkalMini27.Checked = false;
			ribbonCheckBoxMidi14.Checked = false;
			if (ribbonCheckBoxMidi29.Checked == false)
			{
				wizard.ImageResizer.StickPlateHeight = 0;
				wizard.ImageResizer.StickPlateWidth = 0;
			}
			ShowCountPlates();
		}

        private void ribbonCheckBoxBuegelperlen_CheckBoxCheckChanged(object sender, EventArgs e) 
		{
			ribbonCheckBoxSquare.Checked = false;
			ribbonCheckBoxRound.Checked = true;
			ribbonLabelUsedPlates.Visible = ribbonCheckBoxBuegelperlen.Checked;
			ribbonPanelBuegelperlen.Visible = ribbonCheckBoxBuegelperlen.Checked;
			ribbonPanelStiftplatten.Visible = ribbonCheckBoxBuegelperlen.Checked;
			Settings.Default.Stitchplate = ribbonCheckBoxBuegelperlen.Checked;


			//ribbonCheckBoxHamaMini.Visible = ribbonCheckBoxBuegelperlen.Checked;
			//ribbonCheckBoxHamaMidi.Visible = ribbonCheckBoxBuegelperlen.Checked;
			//ribbonCheckBoxHamaMaxi.Visible = ribbonCheckBoxBuegelperlen.Checked;
			ribbonCheckBoxArtkalMini.Visible = ribbonCheckBoxBuegelperlen.Checked;
			ribbonCheckBoxArtkalMidi.Visible = ribbonCheckBoxBuegelperlen.Checked;
		}

        private void ribbonUpDownPlateHeight_DownButtonClicked(object sender, MouseEventArgs e)
        {
			wizard.ImageResizer.StickPlateHeight -= 1;
			ribbonUpDownPlateHeight.TextBoxText = wizard.ImageResizer.StickPlateHeight.ToString();
		}

        private void ribbonUpDownPlateHeight_UpButtonClicked(object sender, MouseEventArgs e)
        {
			wizard.ImageResizer.StickPlateHeight += 1;
			ribbonUpDownPlateHeight.TextBoxText = wizard.ImageResizer.StickPlateHeight.ToString();
		}

        private void ribbonUpDownPlateWidth_DownButtonClicked(object sender, MouseEventArgs e)
        {
			wizard.ImageResizer.StickPlateWidth -= 1;
			ribbonUpDownPlateWidth.TextBoxText = wizard.ImageResizer.StickPlateWidth.ToString();
		}

        private void ribbonUpDownPlateWidth_UpButtonClicked(object sender, MouseEventArgs e)
        {
			wizard.ImageResizer.StickPlateWidth += 1;
			ribbonUpDownPlateWidth.TextBoxText = wizard.ImageResizer.StickPlateWidth.ToString();
		}

        private void ribbonButtonImageEdit_Click(object sender, EventArgs e)
        {
			string originalFile = Settings.Default.UserPalettesLocationSafe + "\\editpic.jpg";
			string editFile = Settings.Default.UserPalettesLocationSafe + "\\editpic1.jpg";

			if (File.Exists(originalFile))
			{
				File.Delete(originalFile);
			}
			
			if (File.Exists(editFile))
			{
				File.Move(editFile, originalFile);
			}
			else
			{
				File.Copy(fileName, originalFile);
			}
			string fileCropped = Settings.Default.UserPalettesLocationSafe + "\\croppedpic.jpg";

			var img = Image.FromFile(originalFile);
			var rect = new Rectangle(new System.Drawing.Point(wizard.ImageCropper.CropRect.Left, wizard.ImageCropper.CropRect.Top), 
				new System.Drawing.Size(wizard.ImageCropper.CropRect.Width, wizard.ImageCropper.CropRect.Height));
			var cloned = new Bitmap(img).Clone(rect, img.PixelFormat);
			var bitmap = new Bitmap(cloned, new System.Drawing.Size(wizard.ImageCropper.CropRect.Width, wizard.ImageCropper.CropRect.Height));
			

			pictureBox1.Image = bitmap;
			pictureBox1.Image.Save(fileCropped);
			
			cloned.Dispose();
			img.Dispose();
			pictureBox1.Dispose();

			/*if (File.Exists(originalFile))
			{
				File.Delete(originalFile);
				File.Copy(fileName, Settings.Default.UserPalettesLocationSafe + "\\editpic.jpg");
			} 
			else
            {
				File.Copy(fileName, Settings.Default.UserPalettesLocationSafe + "\\editpic.jpg");
			}*/

			using (var imageEditDialog = new ImageEditorDialog())
            {
				imageEditDialog.ShowDialog(this);
            }
			LoadSourcePicture();
			

			cloned.Dispose();
			bitmap.Dispose();
			
			pictureBox1.Dispose();
			if (buttonRect.Checked) { buttonRect.Checked = false; }

			//File.Delete(fileCropped);
		}

		private void LoadSourcePicture()
		{
			try
			{
				string fileName = Settings.Default.UserPalettesLocationSafe + "\\editpic1.jpg";
				using (imageBoxSource.Controller?.SuspendUpdateVisualImage())
				{
					if (Path.GetExtension(fileName)?.Equals(".dac", StringComparison.OrdinalIgnoreCase) ?? false)
					{
						CodedImage image;
						using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
						{
							image = ImageSerializer.LoadFromStream(stream, fileName);
						}
						if (!string.IsNullOrEmpty(image?.SourceImageFileName) && File.Exists(image.SourceImageFileName))
						{
							var choice = MessageBox.Show(
								string.Format(Resources.WizardLoadSourceImageInsteadOfScheme, fileName, image.SourceImageFileName),
								Resources.WizardLoadImage, MessageBoxButtons.YesNoCancel);

							switch (choice)
							{
								case DialogResult.Cancel:
									return;
								case DialogResult.Yes:
									fileName = image.SourceImageFileName;
									RestoreImageSettings(image);
									break;
								case DialogResult.No:
									Wizard.ImageSetter.SetNewImage(image);
									RestoreImageSettings(image);
									EnableControls();
									return;
							}
						}
					}

					Wizard.LoadSourceImageFromFile(fileName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(Resources.ErrorCannotOpenFile + Environment.NewLine + ex.Message);
			}
		}
    }
}
