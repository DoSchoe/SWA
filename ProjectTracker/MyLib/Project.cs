using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class Project
    {
        private const char SEPARATOR = '$';
        private const char TIMESPANSEPARATOR = ':';

        private string mProjectName;
        private TimeSpan mTimeEffortProjected;
        private TimeSpan mTimeEffortCurrent;

        public string ProjectName { get => mProjectName; private set => mProjectName = value; }
        public TimeSpan MTimeEffortProjected { get => mTimeEffortProjected; private set => mTimeEffortProjected = value; }
        public TimeSpan MTimeEffortCurrent { get => mTimeEffortCurrent; private set => mTimeEffortCurrent = value; }

        #region CTors
        /// <summary>
        /// Default CTor
        /// </summary>
        public Project()
        {
            mProjectName = String.Empty;
            MTimeEffortProjected = TimeSpan.Zero;
            MTimeEffortCurrent = TimeSpan.Zero;
        }

        /// <summary>
        /// Standard-CTor
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="projectedTimeEffort"></param>
        public Project(string projectName, TimeSpan projectedTimeEffort)
        {
            mProjectName = projectName;
            MTimeEffortProjected = projectedTimeEffort;
            MTimeEffortCurrent = TimeSpan.Zero;
        }

        public Project(string projectName, string currentTimeEffort)
        {
            mProjectName = projectName;
            MTimeEffortProjected = TimeSpan.Zero;
            MTimeEffortCurrent = StringToTimeSpan(currentTimeEffort);
        }

        public Project(string projectString)
        {
            string[] parts = projectString.Split(SEPARATOR);
            mProjectName = parts[0];
            MTimeEffortProjected = StringToTimeSpan(parts[1]);
            MTimeEffortCurrent = StringToTimeSpan(parts[2]);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add Time to project.
        /// </summary>
        /// <param name="time"></param>
        public void AddTime(TimeSpan time)
        {
            MTimeEffortCurrent = MTimeEffortCurrent.Add(time);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(mProjectName);
            sb.Append(SEPARATOR);
            sb.Append(TimeSpanToString(MTimeEffortProjected));
            sb.Append(SEPARATOR);
            sb.Append(TimeSpanToString(MTimeEffortCurrent));
            return sb.ToString();
        }

        public string GetTimeProjected()
        {
            return TimeSpanToString(MTimeEffortProjected);
        }

        public string GetTimeCurrent()
        {
            return TimeSpanToString(MTimeEffortCurrent);
        }
        #endregion

        #region Private Methods
        private string TimeSpanToString(TimeSpan time)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(time.Hours);
            sb.Append(TIMESPANSEPARATOR);
            sb.Append(time.Minutes);
            sb.Append(TIMESPANSEPARATOR);
            sb.Append(time.Seconds);
            return sb.ToString();
        }

        private TimeSpan StringToTimeSpan(string time)
        {
            int hours = Convert.ToInt32(time.Split(TIMESPANSEPARATOR)[0]);
            int minutes = Convert.ToInt32(time.Split(TIMESPANSEPARATOR)[1]);
            int seconds = Convert.ToInt32(time.Split(TIMESPANSEPARATOR)[2]);

            return new TimeSpan(hours, minutes, seconds);
        }
        #endregion
    }
}
