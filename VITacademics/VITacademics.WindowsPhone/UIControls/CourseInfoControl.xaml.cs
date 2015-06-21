﻿using Academics.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VITacademics.Managers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace VITacademics.UIControls
{
    public sealed partial class CourseInfoControl : UserControl, IProxiedControl, INotifyPropertyChanged
    {
        public event EventHandler<RequestEventArgs> ActionRequested;

        private Course _course;

        public Course SelectedCourse
        {
            get { return _course; }
            set
            {
                _course = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedCourse"));
            }
        }

        public CourseInfoControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
#if !DEBUG
            GoogleAnalytics.EasyTracker.GetTracker().SendView("Course Details");
#endif
        }

        public void GenerateView(string parameter)
        {
            try
            {
                SelectedCourse = UserManager.CurrentUser.Courses.Single<Course>((Course c) => string.Equals(c.ClassNumber.ToString(), parameter));
            }
            catch
            {
                SelectedCourse = null;
            }
        }

        public Dictionary<string, object> SaveState()
        {
            return null;
        }

        public void LoadState(Dictionary<string, object> lastState)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
