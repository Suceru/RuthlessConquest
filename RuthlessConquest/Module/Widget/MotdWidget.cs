using System;
using System.Collections.Generic;
using Engine;
using Engine.Input;

namespace Game
{
    public class MotdWidget : CanvasWidget
    {
        public MotdWidget()
        {
            this.m_containerWidget = new CanvasWidget();
            this.Children.Add(this.m_containerWidget);
            MotdManager.MessageOfTheDayUpdated += this.MotdManager_MessageOfTheDayUpdated;
            this.MotdManager_MessageOfTheDayUpdated();
        }

        public override void Update()
        {
            if (Input.Tap != null)
            {
                Widget widget = WidgetsManager.HitTest(Input.Tap.Value);
                if (widget != null && (widget == this || widget.IsChildWidgetOf(this)))
                {
                    this.m_tapsCount++;
                }
            }
            if (this.m_tapsCount >= 5)
            {
                this.m_tapsCount = 0;
                MotdManager.ForceRedownload();
                AudioManager.PlaySound(Sounds.Click, false, 1f, 1f, 0f);
            }
            if (Input.IsKeyDownOnce(Key.PageUp))
            {
                this.GotoLine(this.m_currentLineIndex - 1);
            }
            if (Input.IsKeyDownOnce(Key.PageDown))
            {
                this.GotoLine(this.m_currentLineIndex + 1);
            }
            if (this.m_lines.Count > 0)
            {
                this.m_currentLineIndex %= this.m_lines.Count;
                double realTime = Time.RealTime;
                if (this.m_lastLineChangeTime == 0.0 || realTime - this.m_lastLineChangeTime >= m_lines[m_currentLineIndex].Time)
                {
                    this.GotoLine((this.m_lastLineChangeTime != 0.0) ? (this.m_currentLineIndex + 1) : 0);
                }
                float num = (float)(realTime - this.m_lastLineChangeTime);
                float num2 = (float)(this.m_lastLineChangeTime + m_lines[m_currentLineIndex].Time - 0.33000001311302185 - realTime);
                float x;
                if (num < num2)
                {
                    x = ActualSize.X * (MathUtils.PowSign(MathUtils.Sin(MathUtils.Saturate(1.5f * num) * 3.14159274f / 2f), 0.33f) - 1f);
                }
                else
                {
                    x = ActualSize.X * (1f - MathUtils.PowSign(MathUtils.Sin(MathUtils.Saturate(1.5f * num2) * 3.14159274f / 2f), 0.33f));
                }
                SetWidgetPosition(this.m_containerWidget, new Vector2?(new Vector2(x, 0f)));
                this.m_containerWidget.Size = ActualSize;
                return;
            }
            this.m_containerWidget.Children.Clear();
        }

        public void GotoLine(int index)
        {
            if (this.m_lines.Count > 0)
            {
                this.m_currentLineIndex = MathUtils.Max(index, 0) % this.m_lines.Count;
                this.m_containerWidget.Children.Clear();
                this.m_containerWidget.Children.Add(this.m_lines[this.m_currentLineIndex].Widget);
                this.m_lastLineChangeTime = Time.RealTime;
                this.m_tapsCount = 0;
            }
        }

        public void Restart()
        {
            this.m_currentLineIndex = 0;
            this.m_lastLineChangeTime = 0.0;
        }

        private void MotdManager_MessageOfTheDayUpdated()
        {
            this.m_lines.Clear();
            if (MotdManager.MessageOfTheDay != null)
            {
                foreach (MotdManager.Line line in MotdManager.MessageOfTheDay.Lines)
                {
                    try
                    {
                        MotdWidget.LineData item = this.ParseLine(line);
                        this.m_lines.Add(item);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(string.Format("Error loading MOTD line {0}. Reason: {1}", MotdManager.MessageOfTheDay.Lines.IndexOf(line) + 1, ex.Message));
                    }
                }
            }
            this.Restart();
        }

        private MotdWidget.LineData ParseLine(MotdManager.Line line)
        {
            MotdWidget.LineData lineData = new MotdWidget.LineData();
            lineData.Time = line.Time;
            if (line.Node != null)
            {
                lineData.Widget = WidgetsManager.LoadWidget(null, line.Node, null);
            }
            else
            {
                if (string.IsNullOrEmpty(line.Text))
                {
                    throw new InvalidOperationException("Invalid MOTD line.");
                }
                StackPanelWidget stackPanelWidget = new StackPanelWidget
                {
                    Direction = LayoutDirection.Vertical
                };
                string[] array = line.Text.Replace("\r", "").Split(new char[]
                {
                    '\n'
                });
                for (int i = 0; i < array.Length; i++)
                {
                    string text = array[i].Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        LabelWidget widget = new LabelWidget
                        {
                            Text = text,
                            Font = Fonts.Normal,
                            DropShadow = true
                        };
                        stackPanelWidget.Children.Add(widget);
                    }
                }
                lineData.Widget = stackPanelWidget;
            }
            return lineData;
        }

        private CanvasWidget m_containerWidget;

        private List<MotdWidget.LineData> m_lines = new List<MotdWidget.LineData>();

        private int m_currentLineIndex;

        private double m_lastLineChangeTime;

        private int m_tapsCount;

        private class LineData
        {
            public float Time;

            public Widget Widget;
        }
    }
}
