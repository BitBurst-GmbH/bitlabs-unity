using System;
using UnityEngine;

public class SurveyList
{
    public Survey[] surveys;
}

[Serializable]
public class Survey
{
    public int networkId;
    public int id;
    public string cpi;
    public string value;
    public double loi;
    public int remaining;
    public Details details;
    public int rating;
    public string link;
    public int missingQuestions;

    public void open() 
    {
    }
}

[Serializable]
public class Details
{
    public Category category;
}

[Serializable]
public class Category
{
    public string name;
    public string iconUrl;
}



