﻿using System;
using System.Windows.Controls;
using Timer = System.Threading.Timer;
using HunterPie.Core;

namespace HunterPie.GUI.Widgets.Monster_Widget.Parts {
    /// <summary>
    /// Interaction logic for MonsterPart.xaml
    /// </summary>
    public partial class MonsterPart : UserControl {
        Part Context;
        Timer VisibilityTimer;

        public MonsterPart() {
            InitializeComponent();
        }

        public void SetContext(Part ctx, double MaxHealthBarSize) {
            this.Context = ctx;
            SetPartInformation(MaxHealthBarSize);
            HookEvents();
            StartVisibilityTimer();
        }

        private void HookEvents() {
            Context.OnHealthChange += OnHealthChange;
            Context.OnBrokenCounterChange += OnBrokenCounterChange;
        }

        public void UnhookEvents() {
            Context.OnHealthChange -= OnHealthChange;
            Context.OnBrokenCounterChange -= OnBrokenCounterChange;
            VisibilityTimer?.Dispose();
            this.Context = null;
        }

        private void Dispatch(Action f) {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, f);
        }

        #region Visibility timer
        private void StartVisibilityTimer() {
            if (VisibilityTimer == null) {
                VisibilityTimer = new Timer(_ => HideUnactiveBar(), null, 10000, 0);
            } else {
                VisibilityTimer.Change(10000, 0);
            }
        }

        private void HideUnactiveBar() {
            Dispatch(() => {
                this.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        #endregion

        #region Events
        private void SetPartInformation(double NewSize) {
            this.PartName.Text = $"{Context.Name}";
            this.PartHealth.MaxHealth = Context.TotalHealth;
            this.PartHealth.MaxSize = NewSize - 37;
            this.PartHealth.Health = Context.Health;
            this.PartBrokenCounter.Text = $"{Context.BrokenCounter}";
            this.PartHealthText.Text = $"{Context.Health:0}/{Context.TotalHealth:0}";
        }

        private void OnBrokenCounterChange(object source, MonsterPartEventArgs args) {
            Dispatch(() => {
                this.PartBrokenCounter.Text = $"{args.BrokenCounter}";
            });
        }

        private void OnHealthChange(object source, MonsterPartEventArgs args) {
            Dispatch(() => {
                this.PartHealth.MaxHealth = args.TotalHealth;
                this.PartHealth.Health = args.Health;
                this.PartHealthText.Text = $"{args.Health:0}/{args.TotalHealth:0}";
                this.Visibility = System.Windows.Visibility.Visible;
                StartVisibilityTimer();
            });
        }

        public void UpdateHealthBarSize(double NewSize) {
            if (this.Context == null) return;
            this.PartHealth.MaxSize = NewSize - 37;
            this.PartHealth.MaxHealth = Context.TotalHealth;
            this.PartHealth.Health = Context.Health;
            this.PartHealthText.Text = $"{Context.Health:0}/{Context.TotalHealth:0}";
        }
        #endregion
    }
}
