using UnityEngine;

[CreateAssetMenu(fileName = "GameCalendar", menuName = "RPG/Calendar")]
public class TimeController : ScriptableObject
{
    private int _currentDayOfYear; 
    
    private readonly int _startDay = 274;
    private readonly int _endDay = 365;

    public float GetStoryProgress()
    {
        float progress = (_currentDayOfYear - _startDay) / (_endDay - _startDay);
        return Mathf.Clamp01(progress);
    }
    public void SetStoryProgress(int currentDay){
        _currentDayOfYear = currentDay;
    }
    public void SetStoryProgressInPercents(float progressPercent){
        float allDays = _endDay - _startDay;
        _currentDayOfYear = (int)(allDays * progressPercent) + _startDay;
    }
}