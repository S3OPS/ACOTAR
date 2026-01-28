using System;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Time of day periods
    /// </summary>
    public enum TimeOfDay
    {
        Dawn,        // 5:00 - 7:59
        Morning,     // 8:00 - 11:59
        Midday,      // 12:00 - 14:59
        Afternoon,   // 15:00 - 17:59
        Dusk,        // 18:00 - 19:59
        Evening,     // 20:00 - 22:59
        Night,       // 23:00 - 1:59
        LateNight    // 2:00 - 4:59
    }

    /// <summary>
    /// Seasons in Prythian
    /// Note: Some courts have eternal seasons
    /// </summary>
    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    /// <summary>
    /// Moon phases (important for Fae magic)
    /// </summary>
    public enum MoonPhase
    {
        NewMoon,
        WaxingCrescent,
        FirstQuarter,
        WaxingGibbous,
        FullMoon,
        WaningGibbous,
        LastQuarter,
        WaningCrescent
    }

    /// <summary>
    /// Manages in-game time, day/night cycle, and temporal events
    /// </summary>
    public class TimeSystem : MonoBehaviour
    {
        // Time progression
        public int currentDay = 1;
        public int currentHour = 8;  // Start at morning
        public int currentMinute = 0;
        
        // Time speed (how many in-game minutes per real second)
        public float timeScale = 1.0f;
        private float minuteTimer = 0f;
        
        // Calendar
        public int daysPerMonth = 30;
        public int monthsPerYear = 12;
        public int currentMonth = 1;  // Starting month
        public int currentYear = 1;
        
        // Celestial
        private int moonCycle = 0;  // Days since new moon (0-29)
        
        // Events
        public event Action<TimeOfDay> OnTimeOfDayChanged;
        public event Action<int> OnDayChanged;
        public event Action<Season> OnSeasonChanged;
        public event Action<MoonPhase> OnMoonPhaseChanged;

        private TimeOfDay lastTimeOfDay;
        private Season lastSeason;
        private MoonPhase lastMoonPhase;

        void Start()
        {
            lastTimeOfDay = GetTimeOfDay();
            lastSeason = GetSeason();
            lastMoonPhase = GetMoonPhase();
        }

        void Update()
        {
            if (timeScale > 0)
            {
                AdvanceTime(Time.deltaTime);
            }
        }

        /// <summary>
        /// Advance time based on real-world seconds
        /// </summary>
        private void AdvanceTime(float deltaTime)
        {
            minuteTimer += deltaTime * timeScale;

            if (minuteTimer >= 1.0f)
            {
                int minutesToAdd = Mathf.FloorToInt(minuteTimer);
                minuteTimer -= minutesToAdd;
                
                AddMinutes(minutesToAdd);
            }
        }

        /// <summary>
        /// Add minutes to current time
        /// </summary>
        public void AddMinutes(int minutes)
        {
            currentMinute += minutes;

            while (currentMinute >= 60)
            {
                currentMinute -= 60;
                AddHours(1);
            }

            CheckTimeOfDayChange();
        }

        /// <summary>
        /// Add hours to current time
        /// </summary>
        public void AddHours(int hours)
        {
            currentHour += hours;

            while (currentHour >= 24)
            {
                currentHour -= 24;
                AddDays(1);
            }

            CheckTimeOfDayChange();
        }

        /// <summary>
        /// Add days to current time
        /// </summary>
        public void AddDays(int days)
        {
            currentDay += days;
            moonCycle = (moonCycle + days) % 30;

            OnDayChanged?.Invoke(currentDay);
            CheckMoonPhaseChange();

            while (currentDay > daysPerMonth)
            {
                currentDay -= daysPerMonth;
                AddMonths(1);
            }
        }

        /// <summary>
        /// Add months to current time
        /// </summary>
        private void AddMonths(int months)
        {
            currentMonth += months;
            
            while (currentMonth > monthsPerYear)
            {
                currentMonth -= monthsPerYear;
                currentYear++;
            }

            CheckSeasonChange();
        }

        /// <summary>
        /// Get current time of day
        /// </summary>
        public TimeOfDay GetTimeOfDay()
        {
            if (currentHour >= 5 && currentHour < 8)
                return TimeOfDay.Dawn;
            else if (currentHour >= 8 && currentHour < 12)
                return TimeOfDay.Morning;
            else if (currentHour >= 12 && currentHour < 15)
                return TimeOfDay.Midday;
            else if (currentHour >= 15 && currentHour < 18)
                return TimeOfDay.Afternoon;
            else if (currentHour >= 18 && currentHour < 20)
                return TimeOfDay.Dusk;
            else if (currentHour >= 20 && currentHour < 23)
                return TimeOfDay.Evening;
            else if (currentHour >= 23 || currentHour < 2)
                return TimeOfDay.Night;
            else
                return TimeOfDay.LateNight;
        }

        /// <summary>
        /// Get current season based on month
        /// </summary>
        public Season GetSeason()
        {
            if (currentMonth >= 3 && currentMonth <= 5)
                return Season.Spring;
            else if (currentMonth >= 6 && currentMonth <= 8)
                return Season.Summer;
            else if (currentMonth >= 9 && currentMonth <= 11)
                return Season.Autumn;
            else
                return Season.Winter;
        }

        /// <summary>
        /// Get current moon phase
        /// </summary>
        public MoonPhase GetMoonPhase()
        {
            if (moonCycle < 4)
                return MoonPhase.NewMoon;
            else if (moonCycle < 8)
                return MoonPhase.WaxingCrescent;
            else if (moonCycle < 11)
                return MoonPhase.FirstQuarter;
            else if (moonCycle < 15)
                return MoonPhase.WaxingGibbous;
            else if (moonCycle < 18)
                return MoonPhase.FullMoon;
            else if (moonCycle < 22)
                return MoonPhase.WaningGibbous;
            else if (moonCycle < 26)
                return MoonPhase.LastQuarter;
            else
                return MoonPhase.WaningCrescent;
        }

        /// <summary>
        /// Check if time of day has changed
        /// </summary>
        private void CheckTimeOfDayChange()
        {
            TimeOfDay currentTimeOfDay = GetTimeOfDay();
            if (currentTimeOfDay != lastTimeOfDay)
            {
                lastTimeOfDay = currentTimeOfDay;
                OnTimeOfDayChanged?.Invoke(currentTimeOfDay);
                Debug.Log($"Time of day changed to: {currentTimeOfDay}");
            }
        }

        /// <summary>
        /// Check if season has changed
        /// </summary>
        private void CheckSeasonChange()
        {
            Season currentSeason = GetSeason();
            if (currentSeason != lastSeason)
            {
                lastSeason = currentSeason;
                OnSeasonChanged?.Invoke(currentSeason);
                Debug.Log($"Season changed to: {currentSeason}");
            }
        }

        /// <summary>
        /// Check if moon phase has changed
        /// </summary>
        private void CheckMoonPhaseChange()
        {
            MoonPhase currentPhase = GetMoonPhase();
            if (currentPhase != lastMoonPhase)
            {
                lastMoonPhase = currentPhase;
                OnMoonPhaseChanged?.Invoke(currentPhase);
                Debug.Log($"Moon phase changed to: {currentPhase}");
            }
        }

        /// <summary>
        /// Check if it's currently daytime
        /// </summary>
        public bool IsDaytime()
        {
            return currentHour >= 6 && currentHour < 20;
        }

        /// <summary>
        /// Check if it's currently nighttime
        /// </summary>
        public bool IsNighttime()
        {
            return !IsDaytime();
        }

        /// <summary>
        /// Get magic power multiplier based on moon phase
        /// Full moon enhances Fae magic
        /// </summary>
        public float GetMoonMagicMultiplier()
        {
            MoonPhase phase = GetMoonPhase();
            switch (phase)
            {
                case MoonPhase.FullMoon:
                    return 1.3f;  // 30% boost
                case MoonPhase.WaxingGibbous:
                case MoonPhase.WaningGibbous:
                    return 1.15f; // 15% boost
                case MoonPhase.NewMoon:
                    return 0.85f; // 15% reduction
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get formatted time string
        /// </summary>
        public string GetTimeString()
        {
            return $"{currentHour:D2}:{currentMinute:D2}";
        }

        /// <summary>
        /// Get formatted date string
        /// </summary>
        public string GetDateString()
        {
            return $"Day {currentDay}, Month {currentMonth}, Year {currentYear}";
        }

        /// <summary>
        /// Display current time and celestial information
        /// </summary>
        public void DisplayTimeInfo()
        {
            Debug.Log("\n=== Current Time ===");
            Debug.Log($"Date: {GetDateString()}");
            Debug.Log($"Time: {GetTimeString()}");
            Debug.Log($"Time of Day: {GetTimeOfDay()}");
            Debug.Log($"Season: {GetSeason()}");
            Debug.Log($"Moon Phase: {GetMoonPhase()}");
            Debug.Log($"Moon Magic Multiplier: {GetMoonMagicMultiplier():P0}");
            Debug.Log("===================\n");
        }

        /// <summary>
        /// Set time directly (for debugging/testing)
        /// </summary>
        public void SetTime(int hour, int minute)
        {
            if (hour >= 0 && hour < 24 && minute >= 0 && minute < 60)
            {
                currentHour = hour;
                currentMinute = minute;
                CheckTimeOfDayChange();
            }
        }

        /// <summary>
        /// Set date directly (for debugging/testing)
        /// </summary>
        public void SetDate(int day, int month, int year)
        {
            if (day > 0 && day <= daysPerMonth && month > 0 && month <= monthsPerYear)
            {
                currentDay = day;
                currentMonth = month;
                currentYear = year;
                CheckSeasonChange();
            }
        }

        /// <summary>
        /// Special event: Calanmai (Spring Court fire night)
        /// Occurs on first full moon of spring
        /// </summary>
        public bool IsCalanmai()
        {
            return GetSeason() == Season.Spring && 
                   currentMonth == 3 && 
                   GetMoonPhase() == MoonPhase.FullMoon;
        }

        /// <summary>
        /// Special event: Starfall (Night Court celebration)
        /// Occurs once per year
        /// </summary>
        public bool IsStarfall()
        {
            return currentMonth == 6 && currentDay >= 15 && currentDay <= 17;
        }

        /// <summary>
        /// Check for special Prythian events
        /// </summary>
        public string GetActiveEvent()
        {
            if (IsCalanmai())
                return "Calanmai - The Fire Night";
            else if (IsStarfall())
                return "Starfall";
            else if (GetMoonPhase() == MoonPhase.FullMoon)
                return "Full Moon - Magic Enhanced";
            
            return "None";
        }
    }
}
