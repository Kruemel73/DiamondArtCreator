using System;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Common;
using Ravlyk.SAE.Drawing.Properties;
//using TreeksLicensingLibrary2.EasyIntegration;
using PdfSharp.Internal;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageSizeController : ImageController<ImageSizeManipulator>
	{
		public enum SizeUnit
		{
			Stitch,
			Cm,
			Inch
		}

		public bool isLizenzValid;
		public bool isLizenzDemo;
		public bool isLizenzCommerc;
		
		public ImageSizeController(ImageSizeManipulator manipulator) : base(manipulator)
		{
			SizeLockBy = ImageSizeManipulator.SizeLockType.KeepWidthScaleHeight;

			height = manipulator.SourceImage.Size.Height * width / manipulator.SourceImage.Size.Width;
			if (height == 0) { height = 1; }

			
			isLizenzValid = SAEWizardSettings.Default.isLizenzValid;
			isLizenzDemo = SAEWizardSettings.Default.isLizenzDemo;
			isLizenzCommerc = SAEWizardSettings.Default.isLizenzCommerc;

			using (SuspendCallManipulations())
			{
				RestoreDefaults();
				CallManipulations();
			}
		}
		public bool SquareStones
        {
			get { return squareStones; }
			set
            {
				if (value == squareStones)
                {
					return;
                }
				
				squareStones = value;
				CallManipulations();
			}
        }
		bool squareStones = true;

		public bool RoundStones
		{
			get { return roundStones; }
			set
			{
				if (value == roundStones)
				{
					return;
				}

				roundStones = value;
				CallManipulations();
			}
		}
		bool roundStones = true;
		
		public double StoneSize
        {
			get { return stoneSize; }
            set
            {
				if (value == stoneSize)
                {
					return;
                }
				stoneSize = value;
				CallManipulations();
			}
        }
		double stoneSize = 2.5;

		public double Spu
		{
			get { return spu; }
			set
			{
				if (value == spu)
				{
					return;
				}
				spu = value;
				CallManipulations();
			}
		}

		double spu = 2.5;

		public double StickPlateHeight
		{
			get { return stickPlateHeight; }
			set
			{
				if (value == stickPlateHeight)
				{
					return;
				}
				stickPlateHeight = value;
				
			}
		}

		double stickPlateHeight = 57;

		public double StickPlateWidth
		{
			get { return stickPlateWidth; }
			set
			{
				if (value == stickPlateWidth)
				{
					return;
				}
				stickPlateWidth = value;
			}
		}

		double stickPlateWidth = 57;

		public bool KeepAspect
		{
			get { return keepAspect; }
			set
			{
				if (value == keepAspect)
				{
					return;
				}

				keepAspect = value;
				if (keepAspect)
				{
					if (SizeLockBy == ImageSizeManipulator.SizeLockType.ScaleWidthKeepHeight)
					{
						var x = height;
						height = 0;
						Height = x;
					}
					else
					{
						var x = width;
						width = 0;
						Width = x;
					}
				}
			}
		}
		bool keepAspect = true;
		
		internal ImageSizeManipulator.SizeLockType SizeLockBy { get; set; }

		public ImageResampler.FilterType FilterType
		{
			get { return filterType; }
			set
			{
				if (filterType == value)
				{
					return;
				}

				filterType = value;

				CallManipulations();

				OnPropertyChanged(nameof(FilterType));
			}
		}
		ImageResampler.FilterType filterType = ImageResampler.FilterType.Lanczos3;

		public int Width
		{
			
			get { return width; }
			
			set
			{
				if (!isLizenzValid)
				{
					MaximumSize = 400;
				}
				if ((isLizenzValid && !isLizenzCommerc) && SAEWizardSettings.Default.FT2)
				{
					MaximumSize = 600;
				}
				if (isLizenzCommerc || SAEWizardSettings.Default.FT5)
				{
					MaximumSize = 5000;
				}

				if (value < MinimumSize) { value = MinimumSize; }
				if (value > MaximumSize) { value = MaximumSize; }
				if (width == value)
				{
					return;
				}

				width = value;

				if (KeepAspect)
				{
					height = Manipulator.SourceImage.Size.Height * width / Manipulator.SourceImage.Size.Width;
					if (height == 0)
					{
						height = 1;
					}

					SizeLockBy = ImageSizeManipulator.SizeLockType.KeepWidthScaleHeight;
				}
				else
				{
					SizeLockBy = ImageSizeManipulator.SizeLockType.KeepWidthAndHeight;
				}

				if (height > MaximumSize) { height = MaximumSize; }	
				if (width > MaximumSize) { width = MaximumSize; }

				CallManipulations();

				OnPropertyChanged(nameof(Width));
				OnPropertyChanged(nameof(Height));
				OnPropertyChanged(nameof(SchemeWidth));
				OnPropertyChanged(nameof(SchemeHeight));
			}
		}
		int width = 400;

		public int Height
		{
			get { return height; }
			set
			{
				if (!isLizenzValid)
				{
					MaximumSize = 400;
				}
				if ((isLizenzValid && !isLizenzCommerc) && SAEWizardSettings.Default.FT2)
                {
					MaximumSize = 600;
                }
                if (isLizenzCommerc || SAEWizardSettings.Default.FT5)
                {
					MaximumSize = 5000;
                }

				if (value < MinimumSize) { value = MinimumSize; }
				if (value > MaximumSize) { value = MaximumSize; }
				if (height == value)
				{
					return;
				}

				height = value;

				if (KeepAspect)
				{
					width = Manipulator.SourceImage.Size.Width * height / Manipulator.SourceImage.Size.Height;
					if (width == 0)
					{
						width = 1;
					}

					SizeLockBy = ImageSizeManipulator.SizeLockType.ScaleWidthKeepHeight;
				}
				else
				{
					SizeLockBy = ImageSizeManipulator.SizeLockType.KeepWidthAndHeight;
				}

				if (height > MaximumSize)  { height = MaximumSize; }
				if (width > MaximumSize) { width = MaximumSize;}

				CallManipulations();

				OnPropertyChanged(nameof(Width));
				OnPropertyChanged(nameof(Height));
				OnPropertyChanged(nameof(SchemeWidth));
				OnPropertyChanged(nameof(SchemeHeight));
			}
		}
		int height = 400;

		public  int MinimumSize = 20;
		public  int MaximumSize = 400;
		

		public decimal SchemeWidth
		{
			get { return Width / StitchesPerUnit; }
			set { Width = (int)(value * StitchesPerUnit); }
		}

		public decimal SchemeHeight
		{
			get { return Height / StitchesPerUnit; }
			set { Height = (int)(value * StitchesPerUnit); }
		}

		public SizeUnit Unit
		{
			get { return unit; }
			set
			{
				if (value != unit)
				{
					if (unitForStitchesPerUnit == SizeUnit.Stitch)
					{
						stitchesPerUnit = value == SizeUnit.Inch ? 8.0m : (decimal)spu;
					}

					if (value == SizeUnit.Cm && unitForStitchesPerUnit == SizeUnit.Inch)
					{
						stitchesPerUnit = stitchesPerUnit * 10m / 25.4m;
					}
					else if (value == SizeUnit.Inch && unitForStitchesPerUnit == SizeUnit.Cm)
					{
						stitchesPerUnit = stitchesPerUnit * 25.4m / 10m;
					}

					unit = value;
					if (unit != SizeUnit.Stitch)
					{
						unitForStitchesPerUnit = unit;
					}

					OnPropertyChanged(nameof(Unit));
					OnPropertyChanged(nameof(StitchesPerUnit));
					OnPropertyChanged(nameof(SchemeWidth));
					OnPropertyChanged(nameof(SchemeHeight));
				}
			}
		}
		SizeUnit unit = SizeUnit.Stitch;

		public decimal StitchesPerUnit
		{
			get { return Unit == SizeUnit.Stitch ? 1 : stitchesPerUnit; }
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				if (value != stitchesPerUnit)
				{
					width = (int)(SchemeWidth * value);
					height = (int)(SchemeHeight * value);
					stitchesPerUnit = value;

					CallManipulations();

					OnPropertyChanged(nameof(StitchesPerUnit));
					OnPropertyChanged(nameof(Width));
					OnPropertyChanged(nameof(Height));
					OnPropertyChanged(nameof(SchemeWidth));
					OnPropertyChanged(nameof(SchemeHeight));
				}
			}
		}
		decimal stitchesPerUnit = 1;
		SizeUnit unitForStitchesPerUnit = SizeUnit.Stitch;

		protected override void CallManipulationsCore()
		{
			Manipulator.Resize(new Size(Width, Height), FilterType, SizeLockBy);
		}

		protected override void UpdateValuesFromManipulatedImage()
		{
			base.UpdateValuesFromManipulatedImage();

			width = Manipulator.ManipulatedImage.Size.Width;
			height = Manipulator.ManipulatedImage.Size.Height;

			OnPropertyChanged(nameof(Width));
			OnPropertyChanged(nameof(Height));
			OnPropertyChanged(nameof(SchemeWidth));
			OnPropertyChanged(nameof(SchemeHeight));
		}

		#region Defaults

		protected override void RestoreDefaultsCore()
		{
			using (SuspendCallManipulations())
			{
				KeepAspect = SAEWizardSettings.Default.KeepAspect;
				RoundStones = SAEWizardSettings.Default.RoundStones;
				SquareStones = SAEWizardSettings.Default.SquareStones;
				spu = (double)SAEWizardSettings.Default.Spu;
				stoneSize = (double)SAEWizardSettings.Default.StoneSize;
				stickPlateHeight = SAEWizardSettings.Default.stickPlateHeight;
				stickPlateWidth = SAEWizardSettings.Default.stickPlateWidth;
				Height = SAEWizardSettings.Default.ResizeHeight;
				Width = SAEWizardSettings.Default.ResizeWidth; // Set Width last to have KeepWidthScaleHeight by default

				ImageResampler.FilterType defaultFilterType;
				if (Enum.TryParse(SAEWizardSettings.Default.ResizeFilterType, out defaultFilterType))
				{
					FilterType = defaultFilterType;
				}
			}
		}

		protected override void SaveDefaultsCore()
		{
			SAEWizardSettings.Default.KeepAspect = KeepAspect;
			SAEWizardSettings.Default.RoundStones = RoundStones;
			SAEWizardSettings.Default.SquareStones = SquareStones;
			SAEWizardSettings.Default.Spu = (decimal)spu;
			SAEWizardSettings.Default.StoneSize = (decimal)stoneSize;
			SAEWizardSettings.Default.stickPlateHeight = (int)stickPlateHeight;
			SAEWizardSettings.Default.stickPlateWidth = (int)stickPlateWidth;
			SAEWizardSettings.Default.ResizeWidth = Width;
			SAEWizardSettings.Default.ResizeHeight = Height;
			SAEWizardSettings.Default.ResizeFilterType = FilterType.ToString();
		}

		protected override void RestoreImageSettingsCore(CodedImage image)
		{
			base.RestoreImageSettingsCore(image);

			using (SuspendCallManipulations())
			{
				height = image.Size.Height;
				width = image.Size.Width;
				CallManipulations();
				OnPropertyChanged(nameof(SchemeWidth));
				OnPropertyChanged(nameof(SchemeHeight));

				Unit = SizeUnit.Stitch;
			}
		}

		#endregion
	}
}

