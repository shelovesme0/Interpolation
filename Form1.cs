using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;

namespace CourseWorkRealease
{
    public partial class Form1 : Form
    {
        private bool isCalculated = false;
        private const int MaxPoints = 30;
        private const int MinPoints = 2;
        private const double MinValue = -1e6;
        private const double MaxValue = 1e6;
        private const double minAbsoluteValue = 1e-5;
        private const double MinInterpolationResult= -1e5;
        private const double MaxInterpolationResult = 1e5;

        public Form1()
        {
            InitializeComponentNew();
        }

        private void InitializeComponentNew()
        {
            // Form setup
            Text = "Interpolation Calculator";
            Size = new System.Drawing.Size(1000, 700);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Points input label
            Label lblPoints = new Label
            {
                Text = "Number of Points:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 20)
            };

            // Points input textbox
            TextBox txtNumberOfPoints = new TextBox
            {
                Location = new System.Drawing.Point(120, 20),
                Size = new System.Drawing.Size(60, 20),
                Name = "txtNumberOfPoints"
            };

            // Label for points limit
            Label lblPointsLimit = new Label
            {
                Text = $"({MinPoints} to {MaxPoints} points)",
                Location = new System.Drawing.Point(112, 40),
                AutoSize = true,
                ForeColor = Color.DarkGray
            };

            // Button to initialize grid
            Button btnSetPoints = new Button
            {
                Text = "Set Points",
                Location = new System.Drawing.Point(190, 18),
                Size = new System.Drawing.Size(80, 25),
                Name = "btnSetPoints"
            };

            // DataGridView for points
            DataGridView dgvPoints = new DataGridView
            {
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(243, 180),
                Name = "dgvPoints",
                ColumnCount = 3,
                AllowUserToAddRows = false
            };

            // columns setup
            dgvPoints.Columns[0].HeaderText = "Set #";
            dgvPoints.Columns[0].Width = 50;
            dgvPoints.Columns[0].ReadOnly = true;

            dgvPoints.Columns[1].HeaderText = "X";
            dgvPoints.Columns[1].Width = 66;

            dgvPoints.Columns[2].HeaderText = "Y";
            dgvPoints.Columns[2].Width = 66;

            dgvPoints.Columns[0].Resizable = DataGridViewTriState.False; // "Set #"
            dgvPoints.Columns[1].Resizable = DataGridViewTriState.False; // "X"
            dgvPoints.Columns[2].Resizable = DataGridViewTriState.False; // "Y"

            // Label for coordinates limit
            Label lblCoordinatesLimit = new Label
            {
                Text = $"X, Y range: [{MinValue}, {MaxValue}]\nAbsolute value must be >= {minAbsoluteValue} or 0\nMaximum 6 decimal places",
                Location = new System.Drawing.Point(20, 250),
                AutoSize = true,
                ForeColor = Color.DarkGray
            };

            // X value input
            Label lblXValue = new Label
            {
                Text = "X Value for Interpolation:",
                Location = new System.Drawing.Point(20, 300),
                AutoSize = true
            };

            TextBox txtXValue = new TextBox
            {
                Location = new System.Drawing.Point(150, 300),
                Size = new System.Drawing.Size(60, 20),
                Name = "txtXValue"
            };

            Label lblInterpolationValueLimit = new Label
            {
                Text = $"X interpolation range: [{MinValue}, {MaxValue}]\nY interpolation range: [{MinInterpolationResult}, {MaxInterpolationResult}]\n",
                Location = new System.Drawing.Point(20, 330),
                AutoSize = true,
                ForeColor = Color.DarkGray
            };

            // Method selection
            GroupBox grpMethod = new GroupBox
            {
                Text = "Interpolation Method",
                Location = new System.Drawing.Point(20, 370),
                Size = new System.Drawing.Size(200, 80)
            };

            RadioButton rbLagrange = new RadioButton
            {
                Text = "Lagrange",
                Location = new System.Drawing.Point(10, 20),
                Size = new System.Drawing.Size(80, 20),
                Checked = true,
                Name = "rbLagrange"
            };

            RadioButton rbAitken = new RadioButton
            {
                Text = "Aitken",
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(80, 20),
                Name = "rbAitken"
            };

            // Calculate button
            Button btnCalculate = new Button
            {
                Text = "Calculate",
                Location = new System.Drawing.Point(20, 460),
                Size = new System.Drawing.Size(80, 25),
                Name = "btnCalculate"
            };

            // Save to file button
            Button btnSaveToFile = new Button
            {
                Text = "Save to File",
                Location = new System.Drawing.Point(110, 460),
                Size = new System.Drawing.Size(100, 25),
                Name = "btnSaveToFile",
                Enabled = false
            };

            // Label for interpolation value
            Label lblInterpolationValue = new Label
            {
                Text = "Interpolated Value:",
                Location = new System.Drawing.Point(20, 500),
                AutoSize = true
            };

            // Textbox for interpolation value
            TextBox txtInterpolationValue = new TextBox
            {
                Location = new System.Drawing.Point(20, 520),
                Size = new System.Drawing.Size(200, 20),
                ReadOnly = true,
                Name = "txtInterpolationValue"
            };

            // Label for iterations time
            Label lblIterations = new Label
            {
                Text = "Iterations:",
                Location = new System.Drawing.Point(20, 550),
                AutoSize = true
            };

            // Textbox for iterations
            TextBox txtIterations = new TextBox
            {
                Location = new System.Drawing.Point(20, 570),
                Size = new System.Drawing.Size(200, 20),
                ReadOnly = true,
                Name = "txtIterations"
            };

            // Chart for plotting
            Chart chart = new Chart
            {
                Location = new System.Drawing.Point(300, 20),
                Size = new System.Drawing.Size(640, 500),
                Name = "chartInterpolation"
            };

            ChartArea chartArea = new ChartArea("MainArea");
            chart.ChartAreas.Add(chartArea);
            chart.Series.Add("Points");
            chart.Series.Add("Function");
            chart.Series["Points"].ChartType = SeriesChartType.Point;
            chart.Series["Points"].MarkerStyle = MarkerStyle.Circle;
            chart.Series["Points"].MarkerSize = 8;
            chart.Series["Function"].ChartType = SeriesChartType.Spline;
            chart.Series["Function"].BorderWidth = 2;

            // Label for polynomial
            Label lblPolynomial = new Label
            {
                Text = "Interpolation Polynomial for Lagrange:",
                Location = new System.Drawing.Point(300, 530),
                AutoSize = true
            };

            // Textbox for polynomial
            TextBox txtPolynomial = new TextBox
            {
                Location = new System.Drawing.Point(300, 550),
                Size = new System.Drawing.Size(640, 60),
                Multiline = true,
                ReadOnly = true,
                Name = "txtPolynomial"
            };

            // Add controls to form
            Controls.AddRange(new Control[] {
                lblPoints, txtNumberOfPoints, lblPointsLimit, btnSetPoints,
                dgvPoints, lblCoordinatesLimit, lblXValue, txtXValue,
                grpMethod, btnCalculate, btnSaveToFile,
                lblInterpolationValue, txtInterpolationValue,
                lblIterations, txtIterations,
                chart, lblPolynomial, txtPolynomial, lblInterpolationValueLimit
            });

            // Event to track changes in txtNumberOfPoints
            txtNumberOfPoints.TextChanged += (s, e) =>
            {
                isCalculated = false;
                btnSaveToFile.Enabled = false;
            };

            // Event to track changes in DataGridView
            dgvPoints.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex != 0)
                {
                    isCalculated = false;
                    btnSaveToFile.Enabled = false;
                }
            };

            // Event to track changes in txtXValue
            txtXValue.TextChanged += (s, e) =>
            {
                isCalculated = false;
                btnSaveToFile.Enabled = false;
            };

            // Event to track method selection changes
            rbLagrange.CheckedChanged += (s, e) =>
            {
                isCalculated = false;
                btnSaveToFile.Enabled = false;
            };

            rbAitken.CheckedChanged += (s, e) =>
            {
                isCalculated = false;
                btnSaveToFile.Enabled = false;
            };

            grpMethod.Controls.AddRange(new Control[] { rbLagrange, rbAitken });

            //Event handlers
            btnSetPoints.Click += (s, e) =>
            {
                string input = txtNumberOfPoints.Text.Trim().Replace(',', '.');
                if (input.Length > 1 && input[0] == '0')
                {
                    MessageBox.Show("Number of points cannot start with zero (e.g., 001).", "Invalid input");
                    return;
                }
                if (int.TryParse(input, out int n) && n >= MinPoints && n <= MaxPoints)
                {
                    dgvPoints.RowCount = n;

                    for (int i = 0; i < n; i++)
                    {
                        dgvPoints.Rows[i].Cells[0].Value = i + 1;
                    }

                    isCalculated = false;
                    btnSaveToFile.Enabled = false;
                }
                else if (!int.TryParse(input, out n))
                {
                    MessageBox.Show("Please enter a valid number of points.", "Invalid input");
                }
                else if (n < MinPoints || n > MaxPoints)
                {
                    MessageBox.Show($"Please enter a number between {MinPoints} and {MaxPoints}.", "Invalid input");
                }
            };

            btnCalculate.Click += (s, e) =>
            {
                try
                {
                    // check txtNumberOfPoints
                    string pointsInput = txtNumberOfPoints.Text.Trim().Replace(',', '.');
                    if (pointsInput.Length > 1 && pointsInput[0] == '0')
                    {
                        MessageBox.Show("Number of points cannot start with zero (e.g., 001).", "Invalid input");
                        return;
                    }
                    if (!int.TryParse(pointsInput, out int n) || n < MinPoints || n > MaxPoints)
                    {
                        MessageBox.Show($"Please enter a valid number of points between {MinPoints} and {MaxPoints}.", "Invalid input");
                        return;
                    }

                    // check DataGridView
                    if (dgvPoints.RowCount != n)
                    {
                        MessageBox.Show("Please set the points by clicking 'Set Points' first.", "Set points");
                        return;
                    }

                    Points[] points = new Points[dgvPoints.RowCount];
                    List<double> xValues = new List<double>();

                    for (int i = 0; i < dgvPoints.RowCount; i++)
                    {
                        if (dgvPoints.Rows[i].Cells[1].Value == null || dgvPoints.Rows[i].Cells[2].Value == null)
                        {
                            MessageBox.Show($"Please fill all point coordinates at row {i + 1}.", "Set points");
                            return;
                        }

                        string xStr = dgvPoints.Rows[i].Cells[1].Value.ToString().Trim().Replace(',', '.');
                        string yStr = dgvPoints.Rows[i].Cells[2].Value.ToString().Trim().Replace(',', '.');

                        if (xStr.Length > 1 && xStr[0] == '0' && !xStr.StartsWith("0."))
                        {
                            MessageBox.Show($"X value at row {i + 1} cannot start with zero (e.g., 001).", "Invalid input");
                            return;
                        }

                        if (yStr.Length > 1 && yStr[0] == '0' && !yStr.StartsWith("0."))
                        {
                            MessageBox.Show($"Y value at row {i + 1} cannot start with zero (e.g., 001).", "Invalid input");
                            return;
                        }

                        string[] xParts = xStr.Split('.');
                        if (xParts.Length > 1 && xParts[1].Length > 6 && xParts[1][6] != '0')
                        {
                            MessageBox.Show($"X value at row {i + 1} can't have more than 6 decimal places.", "Invalid input");
                            return;
                        }

                        string[] yParts = yStr.Split('.');
                        if (yParts.Length > 1 && yParts[1].Length > 6 && yParts[1][6] != '0')
                        {
                            MessageBox.Show($"Y value at row {i + 1} can't have more than 6 decimal places.", "Invalid input");
                            return;
                        }

                        if (!double.TryParse(xStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
                        {
                            MessageBox.Show($"X value is invalid format at row {i + 1}. Please enter valid numbers.", "Invalid input");
                            return;
                        }

                        if (!double.TryParse(yStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                        {
                            MessageBox.Show($"Y value is invalid format at row {i + 1}. Please enter valid numbers.", "Invalid input");
                            return;
                        }

                        if (x < MinValue || x > MaxValue)
                        {
                            MessageBox.Show($"X value at row {i + 1} is out of range [{MinValue}, {MaxValue}].", "Invalid input");
                            return;
                        }

                        if (y < MinValue || y > MaxValue)
                        {
                            MessageBox.Show($"Y value at row {i + 1} is out of range [{MinValue}, {MaxValue}].", "Invalid input");
                            return;
                        }

                        if (Math.Abs(x) < minAbsoluteValue && Math.Abs(x) > 0)
                        {
                            MessageBox.Show($"X absolute value at row {i + 1} can't be less than {minAbsoluteValue}.", "Invalid input");
                            return;
                        }

                        if (Math.Abs(y) < minAbsoluteValue && Math.Abs(y) > 0)
                        {
                            MessageBox.Show($"Y absolute value at row {i + 1} can't be less than {minAbsoluteValue}.", "Invalid input");
                            return;
                        }

                        if (xValues.Contains(x))
                        {
                            MessageBox.Show($"Duplicate X value found at row {i + 1}: {x}. Please ensure all X values are unique.", "Invalid input");
                            return;
                        }
                        xValues.Add(x);

                        points[i] = new Points(x, y);
                    }

                    // check txtXValue (X value for interpolation)
                    string xValueInput = txtXValue.Text.Trim().Replace(',', '.');
                    if (string.IsNullOrEmpty(xValueInput))
                    {
                        MessageBox.Show("Please enter a value for X interpolation.", "Invalid input");
                        return;
                    }

                    string[] xInterpolationParts = xValueInput.Split('.');
                    if (xInterpolationParts.Length > 1 && xInterpolationParts[1].Length > 6 && xInterpolationParts[1][6] != '0')
                    {
                        MessageBox.Show($"X value for interpolation can't have more than 6 decimal places.", "Invalid input");
                        return;
                    }

                    if (!double.TryParse(xValueInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double xValue))
                    {
                        MessageBox.Show("Please enter a valid number for X interpolation.", "Invalid input");
                        return;
                    }

                    if (xValue < MinValue || xValue > MaxValue)
                    {
                        MessageBox.Show($"X value for interpolation is out of range [{MinValue}, {MaxValue}].", "Invalid input");
                        return;
                    }

                    if (xValues.Contains(xValue))
                    {
                        MessageBox.Show($"X value {xValue} is already in the points. Please choose a different value.", "Invalid input");
                        return;
                    }

                    // Perform interpolation
                    double result = 0;
                    int iterations = 0;

                    if (rbLagrange.Checked)
                    {
                        (result, iterations) = Interpolation.LagrangeInterpolation(points, xValue);
                    }
                    else
                    {
                        (result, iterations) = Interpolation.AitkenInterpolation(points, xValue);
                    }



                    txtInterpolationValue.Text = $"{result}";
                    txtIterations.Text = $"{iterations}";

                    // Calculate and display polynomial
                    if (rbLagrange.Checked)
                    {
                        string polynomial = GetLagrangePolynomial(points);
                        txtPolynomial.Text = polynomial;
                    }
                    else
                    {
                        txtPolynomial.Text = "Polynomial not available for Aitken method.";
                    }

                    // Plot the graph
                    chart.Series["Points"].Points.Clear();
                    chart.Series["Function"].Points.Clear();

                    if (result > MaxInterpolationResult || result < MinInterpolationResult)
                    {
                        MessageBox.Show("Interpolation result is too big and can't be shown on the graphic, but can be saved into file", "Invalid size");
                    }
                    else
                    {
                        foreach (var point in points)
                        {
                            chart.Series["Points"].Points.AddXY(point.X, point.Y);
                        }

                        double xMin = points.Min(p => p.X);
                        double xMax = points.Max(p => p.X);
                        double range = xMax - xMin;
                        xMin -= range * 0.1;
                        xMax += range * 0.1;
                        int plotPoints = 100;
                        double step = (xMax - xMin) / (plotPoints - 1);

                        for (int i = 0; i < plotPoints; i++)
                        {
                            double x = xMin + i * step;
                            (double y, _) = rbLagrange.Checked
                                ? Interpolation.LagrangeInterpolation(points, x)
                                : Interpolation.AitkenInterpolation(points, x);
                            chart.Series["Function"].Points.AddXY(x, y);
                        }

                        chart.ChartAreas[0].AxisX.Title = "X";
                        chart.ChartAreas[0].AxisY.Title = "Y";
                        chart.ChartAreas[0].AxisX.Minimum = xMin;
                        chart.ChartAreas[0].AxisX.Maximum = xMax;
                        chart.ChartAreas[0].RecalculateAxesScale();
                    }

                    isCalculated = true;
                    btnSaveToFile.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    isCalculated = false;
                    btnSaveToFile.Enabled = false;
                }
            };

            // Event handler for Save to File button
            btnSaveToFile.Click += (s, e) =>
            {
                try
                {
                    if (!isCalculated)
                    {
                        MessageBox.Show("Please calculate the interpolation first.", "Calculate first");
                        return;
                    }

                    Points[] points = new Points[dgvPoints.RowCount];
                    for (int i = 0; i < dgvPoints.RowCount; i++)
                    {
                        string xStr = dgvPoints.Rows[i].Cells[1].Value.ToString().Trim().Replace(',', '.');
                        string yStr = dgvPoints.Rows[i].Cells[2].Value.ToString().Trim().Replace(',', '.');

                        if (!double.TryParse(xStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                            !double.TryParse(yStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                        {
                            MessageBox.Show($"Invalid coordinate format at row {i + 1}. Please enter valid numbers.", "Invalid input");
                            return;
                        }

                        points[i] = new Points(x, y);
                    }

                    string interpolationResult = txtInterpolationValue.Text;
                    string iterations = txtIterations.Text;
                    string polynomial = txtPolynomial.Text;

                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
                        saveFileDialog.Title = "Save Interpolation Results";
                        saveFileDialog.DefaultExt = "txt";
                        saveFileDialog.FileName = "results.txt";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            SaveResultsInFile(points, interpolationResult, iterations, polynomial, saveFileDialog.FileName);
                            MessageBox.Show($"Results saved to {saveFileDialog.FileName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while saving: {ex.Message}");
                }
            };
        }

        private string GetLagrangePolynomial(Points[] points)
        {
            StringBuilder polynomial = new StringBuilder();
            int n = points.Length;

            for (int i = 0; i < n; i++)
            {
                double yi = points[i].Y;
                polynomial.Append(yi >= 0 ? " + " : " - ");
                polynomial.Append(Math.Abs(yi));

                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        double xj = points[j].X;
                        if (xj < 0)
                        {
                            polynomial.Append($" * (x + {Math.Abs(xj)}) / ({points[i].X - Math.Abs(xj)})");
                        }
                        else
                        {
                            polynomial.Append($" * (x - {xj}) / ({points[i].X - xj})");
                        }
                    }
                }
            }

            string result = polynomial.ToString().TrimStart('+', ' ').TrimStart('-', ' ');
            return result;
        }

        private void SaveResultsInFile(Points[] points, string interpolationResult, string iterations, string polynomial, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                foreach (var point in points)
                {
                    writer.WriteLine($"Point: ({point.X}, {point.Y})");
                }
                writer.WriteLine($"Interpolated Value: {interpolationResult}");
                writer.WriteLine($"Iterations: {iterations}");
                if (!string.IsNullOrEmpty(polynomial))
                {
                    writer.WriteLine($"Polynomial: {polynomial}");
                }
                writer.WriteLine("--------------------------------------------------");
            }
        }
    }
}