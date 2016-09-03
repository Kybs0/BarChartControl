using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication11
{
    /// <summary>
    /// BarChartControl.xaml 的交互逻辑
    /// </summary>
    public partial class BarChartControl : UserControl
    {
        public BarChartControl()
        {
            InitializeComponent();
        }

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush",
        typeof(Brush), typeof(BarChartControl),
        new PropertyMetadata(Brushes.Black));

        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness",
        typeof(Thickness), typeof(BarChartControl),
        new PropertyMetadata(new Thickness(1.0, 0.0, 1.0, 1.0)));

        public AxisYModel AxisY
        {
            get { return (AxisYModel)GetValue(AxisYProperty); }
            set { SetValue(AxisYProperty, value); }
        }

        public static readonly DependencyProperty AxisYProperty = DependencyProperty.Register("AxisY",
        typeof(AxisYModel), typeof(BarChartControl),
        new PropertyMetadata(new AxisYModel()));

        public AxisXModel AxisX
        {
            get { return (AxisXModel)GetValue(AxisXProperty); }
            set { SetValue(AxisXProperty, value); }
        }

        public static readonly DependencyProperty AxisXProperty = DependencyProperty.Register("AxisX",
        typeof(AxisXModel), typeof(BarChartControl),
        new PropertyMetadata(new AxisXModel()));
        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }
        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register("HeaderHeight",
        typeof(double), typeof(BarChartControl), new PropertyMetadata(10.0));
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header",
        typeof(string), typeof(BarChartControl), new PropertyMetadata());

        private void BarChartControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            MainBorder.BorderBrush = BorderBrush;
            MainBorder.BorderThickness = BorderThickness;

            BottomGrid.Height = AxisX.Height;
            LeftGrid.Width = AxisY.Width;

            SetYTitlesContent();

            SetXDatasContent();
        }

        private void SetXDatasContent()
        {
            var axisXModel = AxisX;
            if (axisXModel.Datas.Count > 0)
            {
                int count = axisXModel.Datas.Count;
                for (int i = 0; i < count + 1; i++)
                {
                    BottomGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    MainGridFrom0To1.ColumnDefinitions.Add(new ColumnDefinition());
                }
                int index = 0;
                foreach (var data in axisXModel.Datas)
                {
                    //底部
                    var textblock = new TextBlock();
                    textblock.Text = data.Name;
                    textblock.Foreground = axisXModel.ForeGround;
                    textblock.VerticalAlignment = VerticalAlignment.Top;
                    textblock.TextAlignment = TextAlignment.Center;
                    textblock.HorizontalAlignment = HorizontalAlignment.Right;
                    double textBlockWidth = data.LabelWidth;
                    textblock.Width = data.LabelWidth;
                    textblock.Margin = new Thickness(0, 5, -textBlockWidth / 2, 0);
                    Grid.SetColumn(textblock, index);
                    BottomGrid.Children.Add(textblock);


                    //主内容
                    var stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Vertical;

                    var tbl = new TextBlock();
                    tbl.Height = 15;
                    tbl.Margin = new Thickness(0, 0, 0, 5);
                    tbl.Text = data.Value.ToString();
                    tbl.Foreground = axisXModel.ForeGround;
                    tbl.HorizontalAlignment = HorizontalAlignment.Center;
                    stackPanel.Children.Add(tbl);

                    var rectangle = new Rectangle();
                    rectangle.Width = data.BarWidth;
                    double maxValue = AxisY.Titles.Max(i => i.Value);
                    rectangle.Height = (data.Value / maxValue) * (this.ActualHeight - BottomGrid.Height - HeaderHeight);
                    var linearBrush = new LinearGradientBrush()
                    {
                        StartPoint = new Point(1, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection() { 
                            new GradientStop()
                            {
                                Color = data.FillBrush, Offset = 0
                            }, new GradientStop()
                            {
                                Color = data.FillEndBrush, Offset = 1
                            }
                        }
                    };
                    rectangle.Fill = linearBrush;
                    rectangle.HorizontalAlignment = HorizontalAlignment.Center;

                    stackPanel.Children.Add(rectangle);
                    stackPanel.Margin = new Thickness(0, 0, -textBlockWidth / 2, 0);
                    stackPanel.VerticalAlignment = VerticalAlignment.Bottom;
                    stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    Grid.SetColumn(stackPanel, index);
                    MainGridFrom0To1.Children.Add(stackPanel);
                    index++;
                }
            }
        }

        private void SetYTitlesContent()
        {
            var axisYModel = AxisY;
            if (axisYModel.Titles.Count > 0)
            {
                int gridRows = axisYModel.Titles.Count - 1;
                for (int i = 0; i < gridRows; i++)
                {
                    LeftGrid.RowDefinitions.Add(new RowDefinition());
                    MainGridForRow1.RowDefinitions.Add(new RowDefinition());
                }
                int index = 0;
                foreach (var title in axisYModel.Titles)
                {
                    var textblock = new TextBlock();
                    textblock.Text = title.Name;
                    textblock.Foreground = axisYModel.ForeGround;
                    textblock.HorizontalAlignment = HorizontalAlignment.Right;
                    textblock.Height = title.LabelHeight;
                    if (index < gridRows)
                    {
                        textblock.VerticalAlignment = VerticalAlignment.Bottom;
                        textblock.Margin = new Thickness(0, 0, 5, -title.LabelHeight / 2);//因为设置在行底部还不够,必须往下移
                        Grid.SetRow(textblock, gridRows - index - 1);
                    }
                    else
                    {
                        textblock.VerticalAlignment = VerticalAlignment.Top;
                        textblock.Margin = new Thickness(0, -title.LabelHeight / 2, 5, 0);//最后一个,设置在顶部
                        Grid.SetRow(textblock, 0);
                    }
                    LeftGrid.Children.Add(textblock);

                    var border = new Border();
                    border.Height = title.LineHeight;
                    border.BorderBrush = title.LineBrush;
                    double thickness = Convert.ToDouble(title.LineHeight) / 2;
                    border.BorderThickness = new Thickness(0, thickness, 0, thickness);
                    if (index < gridRows)
                    {
                        border.VerticalAlignment = VerticalAlignment.Bottom;
                        border.Margin = new Thickness(0, 0, 0, -thickness);//因为设置在行底部还不够,必须往下移
                        Grid.SetRow(border, gridRows - index - 1);
                    }
                    else
                    {
                        border.VerticalAlignment = VerticalAlignment.Top;
                        border.Margin = new Thickness(0, -thickness, 0, 0);//最后一个,设置在顶部
                        Grid.SetRow(border, 0);
                    }
                    Grid.SetColumn(border, 0);
                    Grid.SetColumnSpan(border, AxisX.Datas.Count + 1);
                    MainGridForRow1.Children.Add(border);
                    index++;
                }
            }
        }
        /// <summary>
        /// 设置分行
        /// </summary>
        /// <param name="leftGrid"></param>
        /// <param name="count"></param>
        private void SetGridRowDefinitions(Grid leftGrid, int count)
        {
            for (int i = 0; i < count; i++)
            {
                leftGrid.RowDefinitions.Add(new RowDefinition());
            }
        }
    }
    public class AxisXModel
    {
        private double _height = 20;
        /// <summary>
        /// 高度
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set { _height = value; }
        }

        private Brush _foreGround = Brushes.Black;
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush ForeGround
        {
            get { return _foreGround; }
            set { _foreGround = value; }
        }

        List<AxisXDataModel> _datas = new List<AxisXDataModel>();
        /// <summary>
        /// 数据
        /// </summary>
        public List<AxisXDataModel> Datas
        {
            get { return _datas; }
            set { _datas = value; }
        }
    }
    public class AxisYModel
    {
        private double _width = 20;
        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get { return _width; } set { _width = value; } }

        private Brush _foreGround = Brushes.Black;
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush ForeGround { get { return _foreGround; } set { _foreGround = value; } }

        List<AxisYDataModel> _titles = new List<AxisYDataModel>();
        /// <summary>
        /// 左侧标题列表
        /// </summary>
        public List<AxisYDataModel> Titles
        {
            get { return _titles; }
            set { _titles = value; }
        }
    }
    public class AxisXDataModel : DataModel
    {
        private double _labelWidth = 20;
        /// <summary>
        /// 底部标签-单个宽度
        /// </summary>
        public double LabelWidth
        {
            get { return _labelWidth; }
            set { _labelWidth = value; }
        }
        private double _barWidth = 20;
        /// <summary>
        /// Bar宽度
        /// </summary>
        public double BarWidth
        {
            get { return _barWidth; }
            set { _barWidth = value; }
        }

        private Color _fillBrush = Colors.Blue;
        /// <summary>
        /// Bar填充颜色
        /// </summary>
        public Color FillBrush
        {
            get
            {
                return _fillBrush;
            }
            set { _fillBrush = value; }
        }

        private Color _fillEndBrush = Colors.Blue;

        public Color FillEndBrush
        {
            get
            {
                return _fillEndBrush;
            }
            set { _fillEndBrush = value; }
        }
    }
    public class AxisYDataModel : DataModel
    {
        private double _labelHeight = 15;
        /// <summary>
        /// 左侧标题栏-单个标题高度
        /// </summary>
        public double LabelHeight
        {
            get { return _labelHeight; }
            set { _labelHeight = value; }
        }
        private double _lineHeight = 0.2;
        /// <summary>
        /// GridLine高度
        /// </summary>
        public double LineHeight
        {
            get { return _lineHeight; }
            set { _lineHeight = value; }
        }

        private Brush _lineBrush = Brushes.Blue;
        /// <summary>
        /// Bar填充颜色
        /// </summary>
        public Brush LineBrush
        {
            get { return _lineBrush; }
            set { _lineBrush = value; }
        }
    }
    public class DataModel
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public double Value { get; set; }
    }
}
