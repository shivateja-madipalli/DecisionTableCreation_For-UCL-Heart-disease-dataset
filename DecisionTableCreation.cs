using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

//    This library is free software; you can redistribute it and/or
//    modify it.
//
//    This library is distributed in the hope that it will be useful,
//    I wrote this library specifically to UCL Heart disease Data Set.
//    Doing alterations to the code can be used for your dataset.
//
namespace DecisionTableCreation_WinForm
{
    public partial class DecisionTableCreation : Form
    {
        public DecisionTableCreation()
        {
            InitializeComponent();
        }

        String[] i;
        List<HeartDiseaseDataSet> HeartDiseaseDataSetList;
        List<HeartDiseaseDataSet> HeartDiseasePresent;
        List<HeartDiseaseDataSet> HeartDiseaseNotPresent;
        List<HeartDiseaseDataSet> TempHeartDiseasePresentForStoringParentNaNObjects;
        List<HeartDiseaseDataSet> TempHeartDiseaseNotPresentForStoringParentNaNObjects;
        HeartDiseaseDataSet data;
        List<DivisionsOfEachPart> AgeHeartAttackPositivePartitionsandValues = new List<DivisionsOfEachPart>();
        DivisionsOfEachPart division;
        float CountToCalculate;
        float InfoGainOfWholeSelectedData;

        float[] Age;
        float[] RestingBP;
        float[] SerunCholestoral;
        float[] FastingBloodSugar;
        float[] MaxHeartRate;
        float[] OldPeak;
        //StringBuilder AttributesToBeBlank;
        List<SingleAttributeValueAndRange> AttributesToBeBlank;// = new ArrayList();

        SingleAttributeValueAndRange ObjForFurtherProcessing;

        List<List<DivisionsOfEachPart>> mainListWithAllSubParts;

        RangesForAllAttributes ObjRangesForAllAttributes;

        MainSubPartsOfAttributesSaving ObjMainSubPartsOfAttributesSaving;
        List<MainSubPartsOfAttributesSaving> LstMainSubPartsOfAttributesSaving = new List<MainSubPartsOfAttributesSaving>();

        //Forsaving Parent Attribute Value
        MainSubPartsOfAttributesSaving MainSubPartsOfAttributesSavingObject = null;

        ArrayList AttributesAlreadydivided = new ArrayList();

        ArrayList ParentAttributes;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                crawlThroughTheData(System.IO.File.ReadAllText(@"D:\CMPE 239\PROJECT\ActualData.txt"));
                CountToCalculate = (70 * (HeartDiseaseDataSetList.Count)) / 100;
                AttributesToBeBlank = new List<SingleAttributeValueAndRange>();
                //AttributesToBeBlank.Add("");
                createTrainingData(CountToCalculate);//, AttributesToBeBlank);
                FindingMaxandMinValueOfEveryAttribute();
                BeforeCreatingSubparts("", float.NaN, float.NaN, "FromFormLoad", null);
                FindingInfoGain(HeartDiseasePresent, HeartDiseaseNotPresent);
                CalculateInfoGainOfSingleAttribute(MainSubPartsOfAttributesSavingObject, "");
                CrawlingThroughOtherLevels();
                //DivingDataSetOnAttributeMaxInfoGain(ObjForFurtherProcessing);
            }
            catch (Exception excep)
            {
                string exception = excep.StackTrace;
            }
        }



        /// <summary>
        /// Crawling through the data and storing them in a list 
        /// </summary>
        public void crawlThroughTheData(String ActualData)
        {
            try
            {
                HeartDiseaseDataSetList = new List<HeartDiseaseDataSet>();
                i = ActualData.Split(new String[] { "\r\n" }, StringSplitOptions.None);
                // foreach (String i in ActualDataSplit)
                for (int j = 0; j < i.Count(); j++)
                {
                    data = new HeartDiseaseDataSet();
                    String[] IndividualData = i[j].Split(new Char[] { ' ' });
                    for (int k = 0; k < IndividualData.Count(); k++)
                    {
                        data.Age = float.Parse(IndividualData[0]);
                        data.Sex = float.Parse(IndividualData[1]);
                        data.ChestPain = float.Parse(IndividualData[2]);
                        data.RestingBloodPressure = float.Parse(IndividualData[3]);
                        data.SerumCholestoral = float.Parse(IndividualData[4]);
                        data.FastingBloodSugar = float.Parse(IndividualData[5]);
                        data.RestingElectrocardiographicResults = float.Parse(IndividualData[6]);
                        data.MaxHeartRate = float.Parse(IndividualData[7]);
                        data.ExerciseIncludeAngina = float.Parse(IndividualData[8]);
                        data.OldPeak = float.Parse(IndividualData[9]);
                        data.SlopeOfThePeakExcercise = float.Parse(IndividualData[10]);
                        data.NumberOfMajorVessels = float.Parse(IndividualData[11]);
                        data.Thal = float.Parse(IndividualData[12]);
                        data.HeartAttackPresence = float.Parse(IndividualData[13]);
                    }
                    HeartDiseaseDataSetList.Add(data);
                }
            }
            catch (Exception e)
            {
                string Exception = e.StackTrace;
            }
        }

        /// <summary>
        /// Going through the training Data and Coming out with some clarity
        /// </summary>
        public void createTrainingData(float countVal)//, ArrayList AttributesToBeBlank)
        {
            try
            {
                HeartDiseasePresent = new List<HeartDiseaseDataSet>();
                HeartDiseaseNotPresent = new List<HeartDiseaseDataSet>();
                HeartDiseaseDataSet tempObj;

                for (int i = 0; i < countVal; i++)
                {
                    tempObj = new HeartDiseaseDataSet();
                    tempObj = HeartDiseaseDataSetList[i];
                    if (tempObj.HeartAttackPresence == 2.0)
                    {
                        HeartDiseasePresent.Add(tempObj);
                    }
                    else
                    {
                        HeartDiseaseNotPresent.Add(tempObj);
                    }
                }
            }
            catch (Exception e)
            {
                String exception = e.StackTrace;
            }
        }



        //public void UpdatingTrainData(float countVal, ArrayList AttributesToBeBlank)
        //{
        //    try
        //    {
        //        //HeartDiseasePresent = new List<HeartDiseaseDataSet>();
        //        //
        //        //HeartDiseaseNotPresent = new List<HeartDiseaseDataSet>();
        //        HeartDiseaseDataSet tempObj;
        //        HeartDiseaseDataSet tempObj1;

        //        for (int j = 0; j < HeartDiseasePresent.Count;j++)
        //        {
        //            tempObj = HeartDiseasePresent[j];
        //        }
        //        for (int k = 0; k < HeartDiseaseNotPresent.Count;k++)
        //        {
        //            tempObj1 = HeartDiseaseNotPresent[k];
        //        }

        //            for (int i = 0; i < countVal; i++)
        //            {
        //                tempObj = new HeartDiseaseDataSet();
        //                tempObj = HeartDiseaseDataSetList[i];
        //                if (AttributesToBeBlank.Contains("Age"))
        //                {
        //                    tempObj.Age = float.NaN;
        //                    tempObj
        //                }
        //                if (AttributesToBeBlank.Contains("Sex"))
        //                {
        //                    tempObj.Sex = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("ChestPain"))
        //                {
        //                    tempObj.ChestPain = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("RestingBloodPressure"))
        //                {
        //                    tempObj.RestingBloodPressure = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("SerumCholestoral"))
        //                {
        //                    tempObj.SerumCholestoral = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("FastingBloodSugar"))
        //                {
        //                    tempObj.FastingBloodSugar = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("RestingElectrocardiographicResults"))
        //                {
        //                    tempObj.RestingElectrocardiographicResults = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("MaxHeartRate"))
        //                {
        //                    tempObj.MaxHeartRate = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("ExerciseIncludeAngina"))
        //                {
        //                    tempObj.ExerciseIncludeAngina = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("OldPeak"))
        //                {
        //                    tempObj.OldPeak = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("SlopeOfThePeakExcercise"))
        //                {
        //                    tempObj.SlopeOfThePeakExcercise = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("NumberOfMajorVessels"))
        //                {
        //                    tempObj.NumberOfMajorVessels = float.NaN;
        //                }
        //                if (AttributesToBeBlank.Contains("Thal"))
        //                {
        //                    tempObj.Thal = float.NaN;
        //                }

        //                if (tempObj.HeartAttackPresence == 2.0)
        //                {
        //                    HeartDiseasePresent.Add(tempObj);
        //                }
        //                else
        //                {
        //                    HeartDiseaseNotPresent.Add(tempObj);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        String exception = e.StackTrace;
        //    }
        //}


        /// <summary>
        /// Finding Maximum and Minimum Value of each and every attribute of the data set provided
        /// in order to divide the data
        /// </summary>
        public void FindingMaxandMinValueOfEveryAttribute()
        {
            try
            {
                ObjRangesForAllAttributes = new RangesForAllAttributes();
                Age = new float[HeartDiseaseDataSetList.Count];
                RestingBP = new float[HeartDiseaseDataSetList.Count];
                SerunCholestoral = new float[HeartDiseaseDataSetList.Count];
                FastingBloodSugar = new float[HeartDiseaseDataSetList.Count];
                MaxHeartRate = new float[HeartDiseaseDataSetList.Count];
                OldPeak = new float[HeartDiseaseDataSetList.Count];
                for (int i = 0; i < HeartDiseaseDataSetList.Count; i++)
                {
                    Age[i] = HeartDiseaseDataSetList[i].Age;
                    RestingBP[i] = HeartDiseaseDataSetList[i].RestingBloodPressure;
                    SerunCholestoral[i] = HeartDiseaseDataSetList[i].SerumCholestoral;
                    //FastingBloodSugar[i] = HeartDiseaseDataSetList[i].FastingBloodSugar;
                    MaxHeartRate[i] = HeartDiseaseDataSetList[i].MaxHeartRate;
                    OldPeak[i] = HeartDiseaseDataSetList[i].OldPeak;
                    Age[i] = HeartDiseaseDataSetList[i].Age;
                }
                for (int i = 0; i < Age.Count(); i++)
                {
                    if (Age[i] == 29)
                    {
                        string sds = "dsdf";
                    }
                }
                Array.Sort(Age);
                ObjRangesForAllAttributes.AgeMaxValue = Age.Max();
                ObjRangesForAllAttributes.AgeMinValue = Age.Min();
                Array.Sort(RestingBP);
                ObjRangesForAllAttributes.RestingBloodPressureMaxVal = RestingBP.Max();
                ObjRangesForAllAttributes.RestingBloodPressureMinVal = RestingBP.Min();
                Array.Sort(SerunCholestoral);
                ObjRangesForAllAttributes.SerumCholestoralMaxValue = SerunCholestoral.Max();
                ObjRangesForAllAttributes.SerumCholestoralMinValue = SerunCholestoral.Min();
                //Array.Sort(FastingBloodSugar);
                //ObjRangesForAllAttributes.FastingBloodSugarMaxVal = FastingBloodSugar.Max();
                //ObjRangesForAllAttributes.FastingBloodSugarMinVal = FastingBloodSugar.Min();
                Array.Sort(MaxHeartRate);
                ObjRangesForAllAttributes.MaxHeartRateMaxVal = MaxHeartRate.Max();
                ObjRangesForAllAttributes.MaxHeartRateMinVal = MaxHeartRate.Min();
                Array.Sort(OldPeak);
                ObjRangesForAllAttributes.OldPeakMaxVal = OldPeak.Max();
                ObjRangesForAllAttributes.OldPeakMinVal = OldPeak.Min();

                //Attributes with just static values
                ObjRangesForAllAttributes.SexValues = new float[] { 0, 1 };
                ObjRangesForAllAttributes.ChestPainValues = new float[] { 1, 2, 3, 4 };
                ObjRangesForAllAttributes.FastingBloodSugarValues = new float[] { 0.0F, 1.0F };
                ObjRangesForAllAttributes.RestingElectrocardiographicResultsValues = new float[] { 0, 1, 2 };
                ObjRangesForAllAttributes.ExerciseIncludeAnginaValues = new float[] { 0.0F, 1.0F };
                ObjRangesForAllAttributes.SlopeOfThePeakExcerciseValues = new float[] { 1, 2, 3 };
                ObjRangesForAllAttributes.NumberOfMajorVesselValues = new float[] { 0, 1, 2, 3 };
                ObjRangesForAllAttributes.ThalValues = new float[] { 3, 6, 7 };

            }
            catch (Exception e)
            {
                String exception = e.StackTrace;
            }
        }

        //Heart Disease Present

        //float[] AgeHeartDiseasePresent;
        //float[] RestingBPHeartDiseasePresent;
        //float[] SerumCholestoralHeartDiseasePresent;
        //float[] MaxHeartRateHeartDiseasePresent;
        //float[] OldPeakHeartDiseasePresent;

        ////The attributes which doesn't require sub divisions
        //float[] SexHeartDiseasePresent;
        //float[] ChestPainHeartDiseasePresent;
        //float[] FastingBloodHeartDiseasePresent;
        //float[] RestingElectrocardiographicResultsHeartDiseasePresent;
        //float[] ExerciseIncludeAnginaHeartDiseasePresent;
        //float[] SlopeOfThePeakExcerciseHeartDiseasePresent;
        //float[] NumberOfMajorVesselsHeartDiseasePresent;
        //float[] ThalHeartDiseasePresent;

        ////Heart Disease Not Present

        //float[] AgeHeartDiseaseNotPresent;
        //float[] RestingBPHeartDiseaseNotPresent;
        //float[] SerumCholestoralHeartDiseaseNotPresent;
        //float[] MaxHeartRateHeartDiseaseNotPresent;
        //float[] OldPeakHeartDiseaseNotPresent;

        ////The attributes which doesn't require sub divisions
        //float[] SexHeartDiseaseNotPresent;
        //float[] ChestPainHeartDiseaseNotPresent;
        //float[] FastingBloodHeartDiseaseNotPresent;
        //float[] RestingElectrocardiographicResultsHeartDiseaseNotPresent;
        //float[] ExerciseIncludeAnginaHeartDiseaseNotPresent;
        //float[] SlopeOfThePeakExcerciseHeartDiseaseNotPresent;
        //float[] NumberOfMajorVesselsHeartDiseaseNotPresent;
        //float[] ThalHeartDiseaseNotPresent;

        ArrayList sds = new ArrayList();

        ArrayList AgeHeartDiseasePresent;
        ArrayList RestingBPHeartDiseasePresent;
        ArrayList SerumCholestoralHeartDiseasePresent;
        ArrayList MaxHeartRateHeartDiseasePresent;
        ArrayList OldPeakHeartDiseasePresent;

        //The attributes which doesn't require sub divisions
        ArrayList SexHeartDiseasePresent;
        ArrayList ChestPainHeartDiseasePresent;
        ArrayList FastingBloodHeartDiseasePresent;
        ArrayList RestingElectrocardiographicResultsHeartDiseasePresent;
        ArrayList ExerciseIncludeAnginaHeartDiseasePresent;
        ArrayList SlopeOfThePeakExcerciseHeartDiseasePresent;
        ArrayList NumberOfMajorVesselsHeartDiseasePresent;
        ArrayList ThalHeartDiseasePresent;

        //Heart Disease Not Present

        ArrayList AgeHeartDiseaseNotPresent;
        ArrayList RestingBPHeartDiseaseNotPresent;
        ArrayList SerumCholestoralHeartDiseaseNotPresent;
        ArrayList MaxHeartRateHeartDiseaseNotPresent;
        ArrayList OldPeakHeartDiseaseNotPresent;

        //The attributes which doesn't require sub divisions
        ArrayList SexHeartDiseaseNotPresent;
        ArrayList ChestPainHeartDiseaseNotPresent;
        ArrayList FastingBloodHeartDiseaseNotPresent;
        ArrayList RestingElectrocardiographicResultsHeartDiseaseNotPresent;
        ArrayList ExerciseIncludeAnginaHeartDiseaseNotPresent;
        ArrayList SlopeOfThePeakExcerciseHeartDiseaseNotPresent;
        ArrayList NumberOfMajorVesselsHeartDiseaseNotPresent;
        ArrayList ThalHeartDiseaseNotPresent;

        /// <summary>
        /// The Whole functionality of the decision tree in embedded in this
        /// </summary>
        public void BeforeCreatingSubparts(String AttributeName, float MaxSingleAttributeSingleValue, float MinSingleAttributeSingleValue, string FromWhere, ArrayList ParentAttributes)
        {
            try
            {
                mainListWithAllSubParts = new List<List<DivisionsOfEachPart>>();

                AgeHeartDiseasePresent = new ArrayList();

                RestingBPHeartDiseasePresent = new ArrayList();

                SexHeartDiseasePresent = new ArrayList();


                ChestPainHeartDiseasePresent = new ArrayList();


                SerumCholestoralHeartDiseasePresent = new ArrayList();


                FastingBloodHeartDiseasePresent = new ArrayList();


                RestingElectrocardiographicResultsHeartDiseasePresent = new ArrayList();

                MaxHeartRateHeartDiseasePresent = new ArrayList();

                ExerciseIncludeAnginaHeartDiseasePresent = new ArrayList();


                OldPeakHeartDiseasePresent = new ArrayList();

                SlopeOfThePeakExcerciseHeartDiseasePresent = new ArrayList();

                NumberOfMajorVesselsHeartDiseasePresent = new ArrayList();

                ThalHeartDiseasePresent = new ArrayList();

                HeartDiseaseDataSet tempHeartDiseasePresentDataSet = null;

                TempHeartDiseasePresentForStoringParentNaNObjects = new List<HeartDiseaseDataSet>();
                TempHeartDiseaseNotPresentForStoringParentNaNObjects = new List<HeartDiseaseDataSet>();

                for (int i = 0; i < HeartDiseasePresent.Count; i++)
                {
                    tempHeartDiseasePresentDataSet = new HeartDiseaseDataSet();
                    //obj = new DivisionsOfEachPart();
                    if (FromWhere == "NotFromFormLoad")
                    {
                        if (AttributeName == "Thal")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].Thal == MaxSingleAttributeSingleValue)
                            {
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "Age")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].Age >= MinSingleAttributeSingleValue && HeartDiseasePresent[i].Age <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.Age = float.NaN;

                                //Add all other attributes
                            }

                        }

                        if (AttributeName == "RestingBP")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].RestingBloodPressure >= MinSingleAttributeSingleValue && HeartDiseasePresent[i].RestingBloodPressure <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);

                                //tempHeartDiseasePresentDataSet.RestingBloodPressure = float.NaN;

                                //Add all other attributes
                            }

                        }

                        if (AttributeName == "SerumCholestoral")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].SerumCholestoral >= MinSingleAttributeSingleValue && HeartDiseasePresent[i].SerumCholestoral <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.SerumCholestoral = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "MaxHeartRate")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].MaxHeartRate >= MinSingleAttributeSingleValue && HeartDiseasePresent[i].MaxHeartRate <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.MaxHeartRate = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "OldPeak")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].OldPeak >= MinSingleAttributeSingleValue && HeartDiseasePresent[i].OldPeak <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.OldPeak = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "Sex")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].Sex == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.Sex = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "ChestPain")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].ChestPain == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.ChestPain = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "FastingBloodSugar")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].FastingBloodSugar == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.FastingBloodSugar = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "RestingElectrocardiographic")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].RestingElectrocardiographicResults == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.RestingElectrocardiographicResults = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "ExerciseIncludeAngina")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].ExerciseIncludeAngina == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.ExerciseIncludeAngina = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "SlopeOfThePeakExcercise")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].SlopeOfThePeakExcercise == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.SlopeOfThePeakExcercise = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "NumberOfMajorVessels")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseasePresent[i].NumberOfMajorVessels == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(HeartDiseasePresent[i]);
                                tempHeartDiseasePresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseasePresent[i], tempHeartDiseasePresentDataSet);
                                tempHeartDiseasePresentDataSet = MakingParentAttributesNaN(tempHeartDiseasePresentDataSet);
                                TempHeartDiseasePresentForStoringParentNaNObjects.Add(tempHeartDiseasePresentDataSet);
                                //tempHeartDiseasePresentDataSet.NumberOfMajorVessels = float.NaN;

                                //Add all other attributes
                            }

                        }
                        if (tempHeartDiseasePresentDataSet.HeartAttackPresence == 2)
                        {
                            AgeHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.Age);
                            RestingBPHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.RestingBloodPressure);
                            SerumCholestoralHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.SerumCholestoral);
                            MaxHeartRateHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.MaxHeartRate);
                            OldPeakHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.OldPeak);
                            SexHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.Sex);
                            ChestPainHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.ChestPain);
                            FastingBloodHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.FastingBloodSugar);
                            RestingElectrocardiographicResultsHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.RestingElectrocardiographicResults);
                            ExerciseIncludeAnginaHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.ExerciseIncludeAngina);
                            SlopeOfThePeakExcerciseHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.SlopeOfThePeakExcercise);
                            NumberOfMajorVesselsHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.NumberOfMajorVessels);
                            ThalHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.Thal);
                        }
                        //else
                        //{
                        //    SexHeartDiseasePresent.Add(float.NaN);
                        //    FastingBloodHeartDiseasePresent.Add(float.NaN);
                        //    RestingElectrocardiographicResultsHeartDiseasePresent.Add(float.NaN);
                        //    ExerciseIncludeAnginaHeartDiseasePresent.Add(float.NaN);
                        //    NumberOfMajorVesselsHeartDiseasePresent.Add(float.NaN);
                        //    OldPeakHeartDiseasePresent.Add(float.NaN);
                        //}
                    }
                    else
                    {
                        tempHeartDiseasePresentDataSet = HeartDiseasePresent[i];

                        AgeHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.Age);
                        RestingBPHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.RestingBloodPressure);
                        SerumCholestoralHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.SerumCholestoral);
                        MaxHeartRateHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.MaxHeartRate);
                        OldPeakHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.OldPeak);
                        SexHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.Sex);
                        ChestPainHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.ChestPain);
                        FastingBloodHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.FastingBloodSugar);
                        RestingElectrocardiographicResultsHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.RestingElectrocardiographicResults);
                        ExerciseIncludeAnginaHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.ExerciseIncludeAngina);
                        SlopeOfThePeakExcerciseHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.SlopeOfThePeakExcercise);
                        NumberOfMajorVesselsHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.NumberOfMajorVessels);
                        ThalHeartDiseasePresent.Add(tempHeartDiseasePresentDataSet.Thal);
                    }
                }

                //float[] arrays used for storing Heart Disease not present
                AgeHeartDiseaseNotPresent = new ArrayList();
                RestingBPHeartDiseaseNotPresent = new ArrayList();
                SerumCholestoralHeartDiseaseNotPresent = new ArrayList();
                MaxHeartRateHeartDiseaseNotPresent = new ArrayList();
                OldPeakHeartDiseaseNotPresent = new ArrayList();


                //DivisionsOfEachPart obj;
                int MinVal = 11;
                int MaxVal = MinVal + 10;

                //The attributes which doesn't require sub divisions
                SexHeartDiseaseNotPresent = new ArrayList();
                ChestPainHeartDiseaseNotPresent = new ArrayList();
                FastingBloodHeartDiseaseNotPresent = new ArrayList();
                RestingElectrocardiographicResultsHeartDiseaseNotPresent = new ArrayList();
                ExerciseIncludeAnginaHeartDiseaseNotPresent = new ArrayList();
                SlopeOfThePeakExcerciseHeartDiseaseNotPresent = new ArrayList();
                NumberOfMajorVesselsHeartDiseaseNotPresent = new ArrayList();
                ThalHeartDiseaseNotPresent = new ArrayList();

                HeartDiseaseDataSet tempHeartDiseaseNotPresentDataSet = null;

                for (int i = 0; i < HeartDiseaseNotPresent.Count; i++)
                {
                    tempHeartDiseaseNotPresentDataSet = new HeartDiseaseDataSet();
                    //obj = new DivisionsOfEachPart();
                    if (FromWhere == "NotFromFormLoad")
                    {
                        if (AttributeName == "Thal")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].Thal == MaxSingleAttributeSingleValue)
                            {

                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.Thal = float.NaN;

                            }

                        }
                        if (AttributeName == "Age")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].Age >= MinSingleAttributeSingleValue && HeartDiseaseNotPresent[i].Age <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.Age = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "RestingBP")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].RestingBloodPressure >= MinSingleAttributeSingleValue && HeartDiseaseNotPresent[i].RestingBloodPressure <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.RestingBloodPressure = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "SerumCholestoral")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].SerumCholestoral >= MinSingleAttributeSingleValue && HeartDiseaseNotPresent[i].SerumCholestoral <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];

                                //HeartDiseaseNotPresent[i].
                                //tempHeartDiseaseNotPresentDataSet.
                                //tempHeartDiseaseNotPresentDataSet.SerumCholestoral = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "MaxHeartRate")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].MaxHeartRate >= MinSingleAttributeSingleValue && HeartDiseaseNotPresent[i].MaxHeartRate <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.MaxHeartRate = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }


                        }
                        if (AttributeName == "OldPeak")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MinSingleAttributeSingleValue != float.NaN && MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].OldPeak >= MinSingleAttributeSingleValue && HeartDiseaseNotPresent[i].OldPeak <= MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.OldPeak = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }


                        }
                        if (AttributeName == "Sex")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].Sex == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.Sex = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }

                        }
                        if (AttributeName == "ChestPain")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].ChestPain == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.ChestPain = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }


                        }
                        if (AttributeName == "FastingBloodSugar")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].FastingBloodSugar == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.FastingBloodSugar = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }


                        }
                        if (AttributeName == "RestingElectrocardiographic")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].RestingElectrocardiographicResults == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.RestingElectrocardiographicResults = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }


                        }
                        if (AttributeName == "ExerciseIncludeAngina")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].ExerciseIncludeAngina == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.ExerciseIncludeAngina = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }


                        }
                        if (AttributeName == "SlopeOfThePeakExcercise")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].SlopeOfThePeakExcercise == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.SlopeOfThePeakExcercise = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }


                        }
                        if (AttributeName == "NumberOfMajorVessels")
                        {
                            //select all required attributes on the basis on this attribute
                            if (MaxSingleAttributeSingleValue != float.NaN && HeartDiseaseNotPresent[i].NumberOfMajorVessels == MaxSingleAttributeSingleValue)
                            {
                                //tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(HeartDiseaseNotPresent[i]);
                                //tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];
                                //tempHeartDiseaseNotPresentDataSet.NumberOfMajorVessels = float.NaN;
                                tempHeartDiseaseNotPresentDataSet = (HeartDiseaseDataSet)CopyPropertyValues(HeartDiseaseNotPresent[i], tempHeartDiseaseNotPresentDataSet);
                                tempHeartDiseaseNotPresentDataSet = MakingParentAttributesNaN(tempHeartDiseaseNotPresentDataSet);
                                TempHeartDiseaseNotPresentForStoringParentNaNObjects.Add(tempHeartDiseaseNotPresentDataSet);
                                //Add all other attributes
                            }


                        }
                        if (tempHeartDiseaseNotPresentDataSet.HeartAttackPresence == 1)
                        {
                            AgeHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.Age);
                            RestingBPHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.RestingBloodPressure);
                            SerumCholestoralHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.SerumCholestoral);
                            MaxHeartRateHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.MaxHeartRate);
                            OldPeakHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.OldPeak);
                            SexHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.Sex);
                            ChestPainHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.ChestPain);
                            FastingBloodHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.FastingBloodSugar);
                            RestingElectrocardiographicResultsHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.RestingElectrocardiographicResults);
                            ExerciseIncludeAnginaHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.ExerciseIncludeAngina);
                            SlopeOfThePeakExcerciseHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.SlopeOfThePeakExcercise);
                            NumberOfMajorVesselsHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.NumberOfMajorVessels);
                            ThalHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.Thal);
                        }
                        //else
                        //{
                        //    SexHeartDiseaseNotPresent.Add(float.NaN);
                        //    FastingBloodHeartDiseaseNotPresent.Add(float.NaN);
                        //    RestingElectrocardiographicResultsHeartDiseaseNotPresent.Add(float.NaN);
                        //    ExerciseIncludeAnginaHeartDiseaseNotPresent.Add(float.NaN);
                        //    NumberOfMajorVesselsHeartDiseaseNotPresent.Add(float.NaN);
                        //    OldPeakHeartDiseaseNotPresent.Add(float.NaN);
                        //}
                    }
                    else
                    {
                        tempHeartDiseaseNotPresentDataSet = HeartDiseaseNotPresent[i];

                        AgeHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.Age);
                        RestingBPHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.RestingBloodPressure);
                        SerumCholestoralHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.SerumCholestoral);
                        MaxHeartRateHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.MaxHeartRate);
                        OldPeakHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.OldPeak);
                        SexHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.Sex);
                        ChestPainHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.ChestPain);
                        FastingBloodHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.FastingBloodSugar);
                        RestingElectrocardiographicResultsHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.RestingElectrocardiographicResults);
                        ExerciseIncludeAnginaHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.ExerciseIncludeAngina);
                        SlopeOfThePeakExcerciseHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.SlopeOfThePeakExcercise);
                        NumberOfMajorVesselsHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.NumberOfMajorVessels);
                        ThalHeartDiseaseNotPresent.Add(tempHeartDiseaseNotPresentDataSet.Thal);
                    }
                }

                //Array.Sort(AgeHeartDiseasePresent);
                AgeHeartDiseasePresent.Sort();
                //Array.Sort(RestingBPHeartDiseasePresent);
                RestingBPHeartDiseasePresent.Sort();
                //Array.Sort(SerumCholestoralHeartDiseasePresent);
                SerumCholestoralHeartDiseasePresent.Sort();
                //Array.Sort(MaxHeartRateHeartDiseasePresent);
                MaxHeartRateHeartDiseasePresent.Sort();
                //Array.Sort(OldPeakHeartDiseasePresent);
                OldPeakHeartDiseasePresent.Sort();
                //Array.Sort(AgeHeartDiseaseNotPresent);
                AgeHeartDiseaseNotPresent.Sort();
                //Array.Sort(RestingBPHeartDiseaseNotPresent);
                RestingBPHeartDiseaseNotPresent.Sort();
                //Array.Sort(SerumCholestoralHeartDiseaseNotPresent);
                SerumCholestoralHeartDiseaseNotPresent.Sort();
                //Array.Sort(MaxHeartRateHeartDiseaseNotPresent);
                MaxHeartRateHeartDiseaseNotPresent.Sort();
                //Array.Sort(OldPeakHeartDiseaseNotPresent);
                OldPeakHeartDiseaseNotPresent.Sort();

                // For Age Sub Dividing
                #region Age Sub Dividing
                float[] returningValue;
                //if (!(AttributesAlreadydivided.Contains("Age")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("Age"))))
                {
                    returningValue = DivdingMaxMinIntoSpecifiedRanges((Age[Age.Count() - 1]), (Age[0]), 10, "Age");
                    //int ValueForMaxValofAge = Age.Count() - 1;
                    List<DivisionsOfEachPart> AgeAttributeValues = new List<DivisionsOfEachPart>();
                    ////Why I have taken Age to send Min Val and Max Val? Yes! in order to make grouping according to the whole min and whole max value than just sending only Heart Attack Positive Min and Max Value
                    //float minValofAge = Age[0];
                    //float maxValofAge = Age[ValueForMaxValofAge];
                    //if (minValofAge % 10.0 == 0)
                    //{
                    //    minValofAge = minValofAge - 1;
                    //}
                    //while (minValofAge % 10.0 != 0)
                    //{
                    //    minValofAge = minValofAge - 1;
                    //}

                    //if (maxValofAge % 10.0 == 0)
                    //{
                    //    maxValofAge = maxValofAge + 1;
                    //}
                    //while (maxValofAge % 10.0 != 0)
                    //{
                    //    maxValofAge = maxValofAge + 1;
                    //}

                    AgeAttributeValues = creationOfSubParts("Age", returningValue, (float[])AgeHeartDiseasePresent.ToArray(typeof(float)), (float[])AgeHeartDiseaseNotPresent.ToArray(typeof(float)));// why dividing with 10? as I need the sub parts with range 10
                    mainListWithAllSubParts.Add(AgeAttributeValues);
                }
                //}
                #endregion
                // END OF For Age Sub Dividing

                //For Resting Blood Pressure
                #region Resting Blood Pressure
                //if (!(AttributesAlreadydivided.Contains("RestingBP")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("RestingBP"))))
                {
                    // int ValueForMaxValofRestingBloodPressure = RestingBP.Count() - 1;
                    List<DivisionsOfEachPart> RestingBpAttributeValues = new List<DivisionsOfEachPart>();
                    returningValue = DivdingMaxMinIntoSpecifiedRanges((RestingBP[RestingBP.Count() - 1]), (RestingBP[0]), 10, "RestingBP");
                    //    float minValofRestingBP = RestingBP[0];
                    //    float maxValofRestingBP = RestingBP[ValueForMaxValofRestingBloodPressure];
                    //    if (minValofRestingBP % 10.0 == 0)
                    //    {
                    //        minValofRestingBP = minValofRestingBP - 1;
                    //    }
                    //    while (minValofRestingBP % 10.0 != 0)
                    //    {
                    //        minValofRestingBP = minValofRestingBP - 1;
                    //    }

                    //    if (maxValofRestingBP % 10.0 == 0)
                    //    {
                    //        maxValofRestingBP = maxValofRestingBP + 1;
                    //    }
                    //    while (maxValofRestingBP % 10.0 != 0)
                    //    {
                    //        maxValofRestingBP = maxValofRestingBP + 1;
                    //    }
                    //    RestingBpAttributeValues = creationOfSubParts("RestingBP", minValofRestingBP - 10, maxValofRestingBP - 9, RestingBPHeartDiseasePresent, RestingBPHeartDiseaseNotPresent, (maxValofRestingBP - minValofRestingBP) / 10, 10); // why dividing with 10? as I need the sub parts with range 10
                    RestingBpAttributeValues = creationOfSubParts("RestingBP", returningValue, (float[])RestingBPHeartDiseasePresent.ToArray(typeof(float)), (float[])RestingBPHeartDiseaseNotPresent.ToArray(typeof(float)));
                    mainListWithAllSubParts.Add(RestingBpAttributeValues);
                }
                //}
                #endregion
                ////END OF For Resting Blood Pressure

                //For SerumCholestoral  Blood Pressure SerumCholestoralHeartDiseaseNotPresent
                #region SerumCholestoral
                //if (!(AttributesAlreadydivided.Contains("SerumCholestoral")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("SerumCholestoral"))))
                {
                    //    int ValueForMaxValofSerumCholestoral = SerunCholestoral.Count() - 1;
                    List<DivisionsOfEachPart> SerumCholestoralAttributeValues = new List<DivisionsOfEachPart>();
                    returningValue = DivdingMaxMinIntoSpecifiedRanges((SerunCholestoral[SerunCholestoral.Count() - 1]), (SerunCholestoral[0]), 50, "SerumCholestoral");
                    //    float minValofSerumCholestoral = SerunCholestoral[0];
                    //    float maxValofSerumCholestoral = SerunCholestoral[ValueForMaxValofSerumCholestoral];
                    //    if (minValofSerumCholestoral % 10.0 == 0)
                    //    {
                    //        minValofSerumCholestoral = minValofSerumCholestoral - 1;
                    //    }
                    //    while (minValofSerumCholestoral % 10.0 != 0)
                    //    {
                    //        minValofSerumCholestoral = minValofSerumCholestoral - 1;
                    //    }

                    //    if (maxValofSerumCholestoral % 10.0 == 0)
                    //    {
                    //        maxValofSerumCholestoral = maxValofSerumCholestoral + 1;
                    //    }
                    //    while (maxValofSerumCholestoral % 10.0 != 0)
                    //    {
                    //        maxValofSerumCholestoral = maxValofSerumCholestoral + 1;
                    //    }
                    //    SerumCholestoralAttributeValues = creationOfSubParts("SerumCholestoral", minValofSerumCholestoral - 50, maxValofSerumCholestoral - 49, SerumCholestoralHeartDiseasePresent, SerumCholestoralHeartDiseaseNotPresent, (maxValofSerumCholestoral - minValofSerumCholestoral) / 50, 50); // why dividing with 50? as I need the sub parts with range 50
                    SerumCholestoralAttributeValues = creationOfSubParts("SerumCholestoral", returningValue, (float[])SerumCholestoralHeartDiseasePresent.ToArray(typeof(float)), (float[])SerumCholestoralHeartDiseaseNotPresent.ToArray(typeof(float)));
                    mainListWithAllSubParts.Add(SerumCholestoralAttributeValues);
                }
                //}
                #endregion
                //END OF For SerumCholestoral

                //For MaxHeartRate MaxHeartRateHeartDiseasePresent
                #region MaxHeartRate
                //int ValueForMaxValofMaxHeart = MaxHeartRate.Count() - 1;
                //if (!(AttributesAlreadydivided.Contains("MaxHeartRate")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("MaxHeartRate"))))
                {
                    List<DivisionsOfEachPart> MaxHeartAttributeValues = new List<DivisionsOfEachPart>();
                    //DivdingMaxMinIntoSpecifiedRanges((Age[Age.Count() - 1]-9), (Age[0]-10), 10,"Age");
                    returningValue = DivdingMaxMinIntoSpecifiedRanges((MaxHeartRate[MaxHeartRate.Count() - 1]), (MaxHeartRate[0]), 20, "MaxHeartRate");
                    //float minValofMaxHeart = MaxHeartRate[0];
                    //float maxValofMaxHeart = MaxHeartRate[ValueForMaxValofMaxHeart];
                    //if (minValofMaxHeart % 10.0 == 0)
                    //{
                    //    minValofMaxHeart = minValofMaxHeart - 1;
                    //}
                    //while (minValofMaxHeart % 10.0 != 0)
                    //{
                    //    minValofMaxHeart = minValofMaxHeart - 1;
                    //}

                    //if (maxValofMaxHeart % 10.0 == 0)
                    //{
                    //    maxValofMaxHeart = maxValofMaxHeart + 1;
                    //}
                    //while (maxValofMaxHeart % 10.0 != 0)
                    //{
                    //    maxValofMaxHeart = maxValofMaxHeart + 1;
                    //}
                    //MaxHeartAttributeValues = creationOfSubParts("MaxHeartRate", minValofMaxHeart - 20, maxValofMaxHeart - 19, MaxHeartRateHeartDiseasePresent, MaxHeartRateHeartDiseaseNotPresent, (maxValofMaxHeart - minValofMaxHeart) / 20, 20); // why dividing with 20? as I need the sub parts with range 20
                    MaxHeartAttributeValues = creationOfSubParts("MaxHeartRate", returningValue, (float[])MaxHeartRateHeartDiseasePresent.ToArray(typeof(float)), (float[])MaxHeartRateHeartDiseaseNotPresent.ToArray(typeof(float)));
                    mainListWithAllSubParts.Add(MaxHeartAttributeValues);
                }
                //}
                #endregion

                //For OldPeak
                #region OldPeak
                //if (!(AttributesAlreadydivided.Contains("OldPeak")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("OldPeak"))))
                {
                    //int ValueFormaxValForOldPeak = OldPeak.Count() - 1;  || !(AttributesAlreadydivided.Contains("MaxHeartRate"))
                    List<DivisionsOfEachPart> OldPeakValue = new List<DivisionsOfEachPart>();
                    //min = (float)Math.Floor(OldPeak[0] - 1);
                    //max = (float)Math.Ceiling(OldPeak[OldPeak.Count() - 1]);
                    float[] returningValue1 = DivdingMaxMinIntoSpecifiedRanges(((float)Math.Ceiling(OldPeak[OldPeak.Count() - 1])), ((float)Math.Floor(OldPeak[0])), 1, "OldPeak");
                    //float minValofOldPeak = OldPeak[0];
                    // float maxValofOldPeak = OldPeak[ValueFormaxValForOldPeak];
                    // minValofOldPeak = (float)Math.Floor(minValofOldPeak);
                    // maxValofOldPeak = (float)Math.Ceiling(maxValofOldPeak);
                    //OldPeakValue = creationOfSubParts("OldPeak", minValofOldPeak - 1, maxValofOldPeak - 0.9F, OldPeakHeartDiseasePresent, OldPeakHeartDiseaseNotPresent, (maxValofOldPeak - minValofOldPeak) / 1, 1); // why dividing with 1.0F? as I need the sub parts with range 1.0F
                    OldPeakValue = creationOfSubParts("OldPeak", returningValue1, (float[])OldPeakHeartDiseasePresent.ToArray(typeof(float)), (float[])OldPeakHeartDiseaseNotPresent.ToArray((typeof(float)))); // why dividing with 1.0F? as I need the sub parts with range 1.0F
                    mainListWithAllSubParts.Add(OldPeakValue);
                }
                //}
                #endregion
                //END OF OldPeak


                //ATTRIBUTES WHICH DOESN'T NEED DIVISIONS
                //END OF For MaxHeartRate


                //For Sex
                #region Sex
                //if (!(AttributesAlreadydivided.Contains("Sex")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("Sex"))))
                {
                    List<DivisionsOfEachPart> SexValues = new List<DivisionsOfEachPart>();
                    SexValues = creationOfParts((float[])SexHeartDiseasePresent.ToArray(typeof(float)), (float[])SexHeartDiseaseNotPresent.ToArray(typeof(float)), new float[] { 0, 1 }, "Sex");
                    mainListWithAllSubParts.Add(SexValues);
                }
                //}
                #endregion
                //END OF Sex

                //For Chest Pain
                #region Chest Pain
                //if (!(AttributesAlreadydivided.Contains("ChestPain")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("ChestPain"))))
                {
                    List<DivisionsOfEachPart> ChestPainValues = new List<DivisionsOfEachPart>();
                    ChestPainValues = creationOfParts((float[])ChestPainHeartDiseasePresent.ToArray(typeof(float)), (float[])ChestPainHeartDiseaseNotPresent.ToArray(typeof(float)), new float[] { 1, 2, 3, 4 }, "ChestPain");
                    mainListWithAllSubParts.Add(ChestPainValues);
                }
                //}
                #endregion
                //END OF Chest Pain

                #region Fasting Blood
                //if (!(AttributesAlreadydivided.Contains("FastingBloodSugar")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("FastingBloodSugar"))))
                {
                    List<DivisionsOfEachPart> FastingBloodValues = new List<DivisionsOfEachPart>();
                    FastingBloodValues = creationOfParts((float[])FastingBloodHeartDiseasePresent.ToArray(typeof(float)), (float[])FastingBloodHeartDiseaseNotPresent.ToArray(typeof(float)), new float[] { 0.0F, 1.0F }, "FastingBloodSugar");
                    mainListWithAllSubParts.Add(FastingBloodValues);
                }
                //}
                #endregion
                //END OF Fasting Blood

                #region Resting Electrocardiographic Results
                //if (!(AttributesAlreadydivided.Contains("RestingElectrocardiographic")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("RestingElectrocardiographic"))))
                {
                    List<DivisionsOfEachPart> RestingElectrocardiographicResultsValue = new List<DivisionsOfEachPart>();
                    RestingElectrocardiographicResultsValue = creationOfParts((float[])RestingElectrocardiographicResultsHeartDiseasePresent.ToArray(typeof(float)), (float[])RestingElectrocardiographicResultsHeartDiseaseNotPresent.ToArray(typeof(float)), new float[] { 0, 1, 2 }, "RestingElectrocardiographic");
                    mainListWithAllSubParts.Add(RestingElectrocardiographicResultsValue);
                }
                // }
                #endregion
                //END OF Resting Electrocardiographic Results


                #region Exercise Include Angina
                //if (!(AttributesAlreadydivided.Contains("ExerciseIncludeAngina")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("ExerciseIncludeAngina"))))
                {
                    List<DivisionsOfEachPart> ExerciseIncludeAnginaValue = new List<DivisionsOfEachPart>();
                    ExerciseIncludeAnginaValue = creationOfParts((float[])ExerciseIncludeAnginaHeartDiseasePresent.ToArray(typeof(float)), (float[])ExerciseIncludeAnginaHeartDiseaseNotPresent.ToArray(typeof(float)), new float[] { 0.0F, 1.0F }, "ExerciseIncludeAngina");
                    mainListWithAllSubParts.Add(ExerciseIncludeAnginaValue);
                }
                // }
                #endregion
                //END OF Exercise Include Angina


                #region Slope Of The Peak Excercise
                //if (!(AttributesAlreadydivided.Contains("SlopeOfThePeakExcercise")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("SlopeOfThePeakExcercise"))))
                {
                    List<DivisionsOfEachPart> SlopeOfThePeakExcerciseHeartValue = new List<DivisionsOfEachPart>();
                    SlopeOfThePeakExcerciseHeartValue = creationOfParts((float[])SlopeOfThePeakExcerciseHeartDiseasePresent.ToArray(typeof(float)), (float[])SlopeOfThePeakExcerciseHeartDiseaseNotPresent.ToArray(typeof(float)), new float[] { 1.0F, 2.0F, 3.0F }, "SlopeOfThePeakExcercise");
                    mainListWithAllSubParts.Add(SlopeOfThePeakExcerciseHeartValue);
                }
                //}
                #endregion
                //END OF Slope Of The Peak Excercise 


                #region Number Of Major Vessels Heart
                //if (!(AttributesAlreadydivided.Contains("NumberOfMajorVessels")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("NumberOfMajorVessels"))))
                {
                    List<DivisionsOfEachPart> NumberOfMajorVesselsValue = new List<DivisionsOfEachPart>();
                    NumberOfMajorVesselsValue = creationOfParts((float[])NumberOfMajorVesselsHeartDiseasePresent.ToArray(typeof(float)), (float[])NumberOfMajorVesselsHeartDiseaseNotPresent.ToArray(typeof(float)), new float[] { 0, 1, 2, 3 }, "NumberOfMajorVessels");
                    mainListWithAllSubParts.Add(NumberOfMajorVesselsValue);
                }
                //}
                #endregion
                //END OF Number Of Major Vessels Heart

                #region Thal
                //if (!(AttributesAlreadydivided.Contains("Thal")))
                //{
                if (FromWhere != "NotFromFormLoad" || (FromWhere == "NotFromFormLoad" && !(ParentAttributes.Contains("Thal"))))
                {
                    List<DivisionsOfEachPart> ThalValue = new List<DivisionsOfEachPart>();
                    ThalValue = creationOfParts((float[])ThalHeartDiseasePresent.ToArray(typeof(float)), (float[])ThalHeartDiseaseNotPresent.ToArray(typeof(float)), new float[] { 3, 6, 7 }, "Thal");
                    mainListWithAllSubParts.Add(ThalValue);
                }
                //}
                #endregion
                //END OF Thal


                #region junk
                ////////////////////////List<DivisionsOfEachPart> ListVal = new List<DivisionsOfEachPart>();
                ////////////////////////DivisionsOfEachPart obj;
                ////////////////////////for(int j=0;j<8;j++)
                ////////////////////////{
                ////////////////////////    obj = new DivisionsOfEachPart();
                ////////////////////////    MinVal = MinVal + 10;
                ////////////////////////    MaxVal = MinVal + 9;
                ////////////////////////    //if (MaxVal <= Age1[Age1.Count() - 1])
                ////////////////////////    //{
                ////////////////////////        var someVal = Age1.Where(item => MinVal <= item);
                ////////////////////////        //Array.ConvertAll<System.Collections.Generic.IEnumerable<float>,float[]>(someVal,)
                ////////////////////////        //someVal.Cast<float[]>;
                ////////////////////////        var some = someVal.Where(item => item <= MaxVal);
                ////////////////////////        //newList.Add(ActualVal);
                ////////////////////////        //newList.Add(creationOfSubParts("Age", MinVal, MaxVal));
                ////////////////////////        obj.Name = "Age";
                ////////////////////////        obj.Value = some.Count();
                ////////////////////////        obj.HeartDisease = true;
                ////////////////////////        obj.MinVal = MinVal;
                ////////////////////////        obj.MaxVal = MaxVal;
                ////////////////////////        ListVal.Add(obj);
                ////////////////////////    //}
                ////////////////////////    //else
                ////////////////////////    //{
                ////////////////////////    //    break;
                ////////////////////////    //}
                ////////////////////////}
                #endregion


            }
            catch (Exception e)
            {
                string Exception = e.StackTrace;
            }
        }

        public object CopyPropertyValues(object source, object destination)
        {
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (destProperty.Name == sourceProperty.Name &&
                destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        destProperty.SetValue(destination, sourceProperty.GetValue(
                            source, new object[] { }), new object[] { });

                        break;
                    }
                }
            }
            return destination;
        }

        public HeartDiseaseDataSet MakingParentAttributesNaN(HeartDiseaseDataSet DataSetObj)
        {
            try
            {
                if (ParentAttributes.Contains("Age"))
                {
                    DataSetObj.Age = float.NaN;
                }
                if (ParentAttributes.Contains("ChestPain"))
                {
                    DataSetObj.ChestPain = float.NaN;
                }
                if (ParentAttributes.Contains("ExerciseIncludeAngina"))
                {
                    DataSetObj.ExerciseIncludeAngina = float.NaN;
                }
                if (ParentAttributes.Contains("FastingBloodSugar"))
                {
                    DataSetObj.FastingBloodSugar = float.NaN;
                }
                if (ParentAttributes.Contains("MaxHeartRate"))
                {
                    DataSetObj.MaxHeartRate = float.NaN;

                }
                if (ParentAttributes.Contains("NumberOfMajorVessels"))
                {
                    DataSetObj.NumberOfMajorVessels = float.NaN;
                }
                if (ParentAttributes.Contains("OldPeak"))
                {
                    DataSetObj.OldPeak = float.NaN;
                }
                if (ParentAttributes.Contains("RestingBP"))
                {
                    DataSetObj.RestingBloodPressure = float.NaN;
                }
                if (ParentAttributes.Contains("RestingElectrocardiographic"))
                {
                    DataSetObj.RestingElectrocardiographicResults = float.NaN;
                }
                if (ParentAttributes.Contains("SerumCholestoral"))
                {
                    DataSetObj.SerumCholestoral = float.NaN;
                }
                if (ParentAttributes.Contains("Sex"))
                {
                    DataSetObj.Sex = float.NaN;
                }
                if (ParentAttributes.Contains("SlopeOfThePeakExcercise"))
                {
                    DataSetObj.SlopeOfThePeakExcercise = float.NaN;
                }
                if (ParentAttributes.Contains("Thal"))
                {
                    DataSetObj.Thal = float.NaN;
                }
                return DataSetObj;
            }
            catch (Exception e)
            {
                String ex = e.StackTrace;
                return null;
            }
        }

        /// <summary>
        /// This takes the Attribute as input parameter and stores each division accordingly.
        /// </summary>
        /// <param name="nameOfTheAttribute"></param>
        /// <returns></returns>
        public List<DivisionsOfEachPart> creationOfSubParts(String nameOfTheAttribute, float[] ArrayWithRangeOfValues, float[] HeartDiseasePositiveArray, float[] HeartDiseaseNotPositiveArray)
        {
            try
            {
                List<DivisionsOfEachPart> ListVal = new List<DivisionsOfEachPart>();
                DivisionsOfEachPart HeartAttackPositiveobj;
                DivisionsOfEachPart HeartAttackNegitiveobj;
                for (int j = 0, k = 1; j < ArrayWithRangeOfValues.Count(); j = j + 2, k = k + 2)
                {
                    HeartAttackPositiveobj = new DivisionsOfEachPart();
                    HeartAttackNegitiveobj = new DivisionsOfEachPart();
                    //if (nameOfTheAttribute != "OldPeak")
                    //{
                    //    //MinVal = MinVal + RangeforPartition;
                    //    //MaxVal = MinVal + (RangeforPartition - 1);
                    //}
                    //else
                    //{
                    //    //MinVal = MinVal + 1F;
                    //    //MaxVal = MinVal + 1F;
                    //}
                    //HeartAttackPositive Calculation
                    //HeartDiseasePositiveArray.
                    var someVal = HeartDiseasePositiveArray.Where(item => ArrayWithRangeOfValues[j] <= item);
                    var some = someVal.Where(item => item <= ArrayWithRangeOfValues[k]);
                    HeartAttackPositiveobj.Name = nameOfTheAttribute;
                    HeartAttackPositiveobj.Value = some.Count();
                    HeartAttackPositiveobj.HeartDisease = true;
                    HeartAttackPositiveobj.MinVal = ArrayWithRangeOfValues[j];
                    HeartAttackPositiveobj.MaxVal = ArrayWithRangeOfValues[k];
                    ListVal.Add(HeartAttackPositiveobj);

                    //HeartAttackNegitive Calculation
                    var someVal1 = HeartDiseaseNotPositiveArray.Where(item => ArrayWithRangeOfValues[j] <= item);
                    var some1 = someVal1.Where(item => item <= ArrayWithRangeOfValues[k]);
                    HeartAttackNegitiveobj.Name = nameOfTheAttribute;
                    HeartAttackNegitiveobj.Value = some1.Count();
                    HeartAttackNegitiveobj.HeartDisease = false;
                    HeartAttackNegitiveobj.MinVal = ArrayWithRangeOfValues[j];
                    HeartAttackNegitiveobj.MaxVal = ArrayWithRangeOfValues[k];
                    ListVal.Add(HeartAttackNegitiveobj);
                }
                return ListVal;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// This method is for those attributes which doesn't need sub divisions of the values
        /// </summary>
        /// <returns></returns>
        public List<DivisionsOfEachPart> creationOfParts(float[] HeartDiseasePositiveArray, float[] HeartDiseaseNotPositiveArray, float[] value, String nameOfTheAttribute)
        {
            try
            {
                List<DivisionsOfEachPart> ListVal = new List<DivisionsOfEachPart>();
                DivisionsOfEachPart HeartAttackPositiveobj;
                DivisionsOfEachPart HeartAttackNegitiveobj;
                for (int g = 0; g < value.Count(); g++)
                {
                    HeartAttackPositiveobj = new DivisionsOfEachPart();
                    HeartAttackNegitiveobj = new DivisionsOfEachPart();
                    //individually finding how many are men and women
                    var someVal = HeartDiseasePositiveArray.Where(item => value[g] == item);
                    //Sample Expressions
                    HeartAttackPositiveobj.Name = nameOfTheAttribute;
                    HeartAttackPositiveobj.Value = someVal.Count();
                    HeartAttackPositiveobj.MaxVal = value[g];
                    HeartAttackPositiveobj.HeartDisease = true;
                    ListVal.Add(HeartAttackPositiveobj);

                    //end of sample expressions

                    var someVal1 = HeartDiseaseNotPositiveArray.Where(item => value[g] == item);
                    HeartAttackNegitiveobj.Name = nameOfTheAttribute;
                    HeartAttackNegitiveobj.Value = someVal1.Count();
                    HeartAttackNegitiveobj.MaxVal = value[g];
                    HeartAttackNegitiveobj.HeartDisease = false;
                    ListVal.Add(HeartAttackNegitiveobj);

                }

                return ListVal;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Calculating Information Gain of whole dataset selected
        /// </summary>
        public void FindingInfoGain(List<HeartDiseaseDataSet> HeartDiseasePresentObj, List<HeartDiseaseDataSet> HeartDiseaseNotPresentObj)
        {
            // {-(prob of No * ( log (prob of no) base 2)) + (prob of Yes * ( log (prob of yes) base 2))}
            //object re = HeartDiseaseNotPresent.Where(item => item.);

            CountToCalculate = HeartDiseaseNotPresentObj.Count() + HeartDiseasePresentObj.Count();
            float ProbOfNo = (HeartDiseaseNotPresentObj.Count() / CountToCalculate);
            float ProbOfYes = (HeartDiseasePresentObj.Count() / CountToCalculate);
            InfoGainOfWholeSelectedData = (float)(-(ProbOfNo * (Math.Log(ProbOfNo, 2)) + ProbOfYes * (Math.Log(ProbOfYes, 2))));
        }

        public List<HeartDiseaseDataSet> RefiningDiseasePresentAndNotPresentDataSet(List<HeartDiseaseDataSet> HeartDiseaseDataSetObj, String AttributePropName)//, float MaxValue, float MinValue)
        {
            try
            {
                if (AttributePropName == "Age")
                {
                    //var someVal = HeartDiseaseDataSetObj.Where(item => MinValue <= item.Age);
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)someVal.Where(item => item.Age <= MaxValue).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(float.NaN)).ToList();
                }
                else if (AttributePropName == "ChestPain")
                {
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => MaxValue == item.ChestPain).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.ChestPain)).ToList();
                    //var some = someVal.Where(item => item.Age <= MaxValue);
                }
                else if (AttributePropName == "ExerciseIncludeAngina")
                {
                    //has 0 value
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => MaxValue == item.ExerciseIncludeAngina).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.ExerciseIncludeAngina)).ToList();
                    //var some = someVal.Where(item => item.Age <= MaxValue);
                }
                else if (AttributePropName == "FastingBloodSugar")
                {
                    //has 0 value
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => MaxValue == item.FastingBloodSugar).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.FastingBloodSugar)).ToList();
                    //var some = someVal.Where(item => item.Age <= MaxValue);
                }
                else if (AttributePropName == "MaxHeartRate")
                {
                    //var someVal = HeartDiseaseDataSetObj.Where(item => MinValue <= item.MaxHeartRate);
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)someVal.Where(item => item.MaxHeartRate <= MaxValue).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.MaxHeartRate)).ToList();
                }
                else if (AttributePropName == "NumberOfMajorVessels")
                {
                    //Has 0 value
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => MaxValue == item.NumberOfMajorVessels).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.NumberOfMajorVessels)).ToList();
                    //var some = someVal.Where(item => item.Age <= MaxValue);
                }
                if (AttributePropName == "OldPeak")
                {
                    //Check
                    //var someVal = HeartDiseaseDataSetObj.Where(item => MinValue <= item.OldPeak);
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)someVal.Where(item => item.OldPeak <= MaxValue).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.OldPeak)).ToList();
                }
                else if (AttributePropName == "RestingBP")
                {
                    //var someVal = HeartDiseaseDataSetObj.Where(item => MinValue <= item.RestingBloodPressure);
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)someVal.Where(item => item.RestingBloodPressure <= MaxValue).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.RestingBloodPressure)).ToList();
                }
                else if (AttributePropName == "RestingElectrocardiographic")
                {
                    // has 0 value
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => MaxValue == item.RestingElectrocardiographicResults).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.RestingElectrocardiographicResults)).ToList();
                    // var some = someVal.Where(item => item.Age <= MaxValue);
                }
                if (AttributePropName == "SerumCholestoral")
                {
                    //var someVal = HeartDiseaseDataSetObj.Where(item => MinValue <= item.SerumCholestoral);
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)someVal.Where(item => item.SerumCholestoral <= MaxValue).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.SerumCholestoral)).ToList();

                }
                else if (AttributePropName == "Sex")
                {
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => MaxValue == item.Sex).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.Sex)).ToList();
                    //var some = someVal.Where(item => item.Age <= MaxValue);
                }
                else if (AttributePropName == "SlopeOfThePeakExcercise")
                {
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => MaxValue == item.SlopeOfThePeakExcercise).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.SlopeOfThePeakExcercise)).ToList();
                    //var some = someVal.Where(item => item.Age <= MaxValue);
                }
                else if (AttributePropName == "Thal")
                {
                    //HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => MaxValue == item.Thal).ToList();
                    HeartDiseaseDataSetObj = (List<HeartDiseaseDataSet>)HeartDiseaseDataSetObj.Where(item => float.IsNaN(item.Thal)).ToList();
                    // var some = someVal.Where(item => item. == MaxValue);
                }
                return HeartDiseaseDataSetObj;
            }
            catch (Exception e)
            {
                string exce = e.StackTrace;
                return null;
            }

        }

        //
        /// <summary>
        ///ParentObject : MainSubPartsOfAttributesSavingObjectAsParentOfCurrentAttributeBeingProcessed  
        /// </summary>
        /// <param name="ParentObject">MainSubPartsOfAttributesSavingObjectAsParentOfCurrentAttributeBeingProcessed </param>
        public MainSubPartsOfAttributesSaving CalculateInfoGainOfSingleAttribute(MainSubPartsOfAttributesSaving ParentObject, String ParentAttributeRangeValue)
        {
            try
            {
                ObjMainSubPartsOfAttributesSaving = new MainSubPartsOfAttributesSaving();
                List<DivisionsOfEachPart> tempList = new List<DivisionsOfEachPart>();
                DivisionsOfEachPart tempObj;
                SingleAttributeInfoGainValue AttributeInfoGain;
                ObjForFurtherProcessing = new SingleAttributeValueAndRange();
                float TotalProbOfSingleAttribute;
                float PositiveProbOfSingleAttribute = float.NaN;
                float NegitiveProbOfSingleAttribute = float.NaN;
                float InfoOfSingleAttribute = 0;
                List<SingleAttributeInfoGainValue> AttributeInfo = new List<SingleAttributeInfoGainValue>();
                AttributeRangesAndHeartDiseaseDicisionValues AttributeRangesAndHeartDiseaseDicisionValuesObj;
                for (int i = 0; i < mainListWithAllSubParts.Count; i++)
                {

                    AttributeInfoGain = new SingleAttributeInfoGainValue();
                    InfoOfSingleAttribute = 0;
                    //
                    for (int j = 0; j < mainListWithAllSubParts[i].Count; j = j + 2)
                    {
                        AttributeRangesAndHeartDiseaseDicisionValuesObj = new AttributeRangesAndHeartDiseaseDicisionValues();
                        tempObj = new DivisionsOfEachPart();
                        tempObj = mainListWithAllSubParts[i].Find(o => o.MaxVal == mainListWithAllSubParts[i][j].MaxVal && o.HeartDisease == !mainListWithAllSubParts[i][j].HeartDisease);
                        if (mainListWithAllSubParts[i][j].Value != 0.0 || tempObj.Value != 0.0)
                        {
                            TotalProbOfSingleAttribute = (mainListWithAllSubParts[i][j].Value + tempObj.Value) / CountToCalculate;
                            PositiveProbOfSingleAttribute = mainListWithAllSubParts[i][j].Value / (mainListWithAllSubParts[i][j].Value + tempObj.Value);
                            NegitiveProbOfSingleAttribute = tempObj.Value / (mainListWithAllSubParts[i][j].Value + tempObj.Value);
                            if (float.IsNaN((float)(TotalProbOfSingleAttribute * (-(NegitiveProbOfSingleAttribute * (Math.Log(NegitiveProbOfSingleAttribute, 2)) + PositiveProbOfSingleAttribute * (Math.Log(PositiveProbOfSingleAttribute, 2)))))))
                            {

                            }
                            else
                            {
                                InfoOfSingleAttribute = InfoOfSingleAttribute + (float)(TotalProbOfSingleAttribute * (-(NegitiveProbOfSingleAttribute * (Math.Log(NegitiveProbOfSingleAttribute, 2)) + PositiveProbOfSingleAttribute * (Math.Log(PositiveProbOfSingleAttribute, 2)))));
                            }
                            AttributeRangesAndHeartDiseaseDicisionValuesObj = new AttributeRangesAndHeartDiseaseDicisionValues();
                            AttributeRangesAndHeartDiseaseDicisionValuesObj.AttributeName = tempObj.Name;
                            AttributeRangesAndHeartDiseaseDicisionValuesObj.MaxValue = mainListWithAllSubParts[i][j].MaxVal;
                            AttributeRangesAndHeartDiseaseDicisionValuesObj.MinValue = mainListWithAllSubParts[i][j].MinVal;
                            AttributeRangesAndHeartDiseaseDicisionValuesObj.PositiveProbOfSingleAttribute = PositiveProbOfSingleAttribute;
                            AttributeRangesAndHeartDiseaseDicisionValuesObj.NegitiveProbOfSingleAttribute = NegitiveProbOfSingleAttribute;
                            if (PositiveProbOfSingleAttribute == 0.0F || float.IsNaN(PositiveProbOfSingleAttribute))
                            {
                                AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseasePositiveEnd = false;
                                AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseaseNegitiveEnd = true;
                            }
                            if (NegitiveProbOfSingleAttribute == 0.0F || float.IsNaN(NegitiveProbOfSingleAttribute))
                            {
                                AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseaseNegitiveEnd = false;
                                AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseasePositiveEnd = true;
                            }
                            if (PositiveProbOfSingleAttribute == 0 || NegitiveProbOfSingleAttribute == 0)
                            {
                                //I have a doubt in this, 
                                // when are we suppose to end the loop? mean when are we going to stop finding Info gain
                                //as per knowledge I have, we need to stop when ever we get a YES/NO for heart disease,
                                //get it clarified

                                String sdsd = "asdsa";
                            }
                        }
                        AttributeInfoGain.AttributeName = mainListWithAllSubParts[i][j].Name;
                        AttributeInfoGain.AttributeRangesAndHeartDiseaseDicisionValuesObj = AttributeRangesAndHeartDiseaseDicisionValuesObj;
                    }

                    AttributeInfoGain.InformationValue = InfoGainOfWholeSelectedData - InfoOfSingleAttribute;
                    AttributeInfo.Add(AttributeInfoGain);
                    //if (AttributeInfoGain.InformationValue == InfoGainOfWholeSelectedData)
                    //{
                    //    ObjMainSubPartsOfAttributesSaving.ReachedEnd = true;
                    //    // ObjMainSubPartsOfAttributesSaving.HeartDiseasePositiveValue = 
                    //}
                    //else
                    //{
                    //    ObjMainSubPartsOfAttributesSaving.ReachedEnd = false;
                    //}
                }
                SingleAttributeInfoGainValue MaxValue = new SingleAttributeInfoGainValue();
                // MaxValue.InformationValue = AttributeInfo.Max(o => o.InformationValue);
                float Val = AttributeInfo.Max(o => o.InformationValue);
                MaxValue = AttributeInfo.Find(o => o.InformationValue == Val);

                float Val1 = AttributeInfo.Min(o => o.InformationValue);
                if (Val1 == 0)
                {
                    SingleAttributeInfoGainValue MinValue = new SingleAttributeInfoGainValue();
                    MinValue = AttributeInfo.Find(o => o.InformationValue == Val1);
                }

                //SingleAttributeValueAndRange Value = new SingleAttributeValueAndRange();
                //Value.RangeOfValues = 
                //DivingDataSetOnAttributeMaxInfoGain(MaxValue);
                object[] ReturnValue = AccessingPropertiesOfAllClasses(MaxValue.AttributeName, ObjRangesForAllAttributes);
                //objarr.Cast<int>().ToArray();
                //ArrayConverter cnv;
                //cnv.ConvertTo(ReturnValue,System.Type.GetTypeArray();
                //for (int i = 0; i < ReturnValue.Count(); i++)
                //{
                //    //(float)Convert.ChangeType(reader["Prijs"], typeof(float));
                //    ObjForFurtherProcessing.RangeOfValues[i] = (float)Convert.ChangeType(ReturnValue[i], typeof(float));
                //        //(int)ReturnValue[i];
                //}
                ObjForFurtherProcessing.RangeOfValues = ReturnValue.Cast<float>().ToArray();//).Cast<float>().ToArray();
                ObjForFurtherProcessing.Name = MaxValue.AttributeName;
                AttributesToBeBlank.Add(ObjForFurtherProcessing);
                ObjMainSubPartsOfAttributesSaving.mainListWithAllSubParts = mainListWithAllSubParts;
                ObjMainSubPartsOfAttributesSaving.SingleAttributeValueAndRangeObject = ObjForFurtherProcessing;
                ObjMainSubPartsOfAttributesSaving.SingleAttributeInfoGainValueObject = MaxValue;
                if (ParentObject != null && ParentAttributeRangeValue != null)
                {
                    ObjMainSubPartsOfAttributesSaving.ParentAttributeValue = ParentObject;
                    ObjMainSubPartsOfAttributesSaving.ParentAttributeRangeValue = ParentAttributeRangeValue;
                    ObjMainSubPartsOfAttributesSaving.LevelNumber = ParentObject.LevelNumber + 1;
                }
                else
                {
                    ObjMainSubPartsOfAttributesSaving.LevelNumber = 0;
                }
                LstMainSubPartsOfAttributesSaving.Add(ObjMainSubPartsOfAttributesSaving);
                AttributesAlreadydivided.Add(MaxValue.AttributeName);
                return ObjMainSubPartsOfAttributesSaving;
            }
            catch (Exception e)
            {
                String error = e.StackTrace;
                return null;
            }
        }

        public List<MainSubPartsOfAttributesSaving> DivingDataSetOnAttributeMaxInfoGain(MainSubPartsOfAttributesSaving ParentAttributeMainSubPartsOfAttributesSaving, SingleAttributeValueAndRange AttributeVal, String GrandParentValue, ArrayList ParentAttributes)
        {
            MainSubPartsOfAttributesSavingObject = new MainSubPartsOfAttributesSaving();
            MainSubPartsOfAttributesSavingObject.SingleAttributeValueAndRangeObject = AttributeVal;
            List<MainSubPartsOfAttributesSaving> returningValue = new List<MainSubPartsOfAttributesSaving>();
            //TreeNode tempNode;
            
            if (AttributeVal.Name == "Age" || AttributeVal.Name == "MaxHeartRate" || AttributeVal.Name == "OldPeak" || AttributeVal.Name == "RestingBP" || AttributeVal.Name == "SerumCholestoral")
            {
                
                for (int i = 0, j = 1; i < AttributeVal.RangeOfValues.Count(); i = i + 2, j = j + 2)
                {

                    if (AttributeVal.RangeOfValues.Count() > 2)
                    {
                        //tempNode = new TreeNode();
                        //Add parent Attributes Parent Value too
                        String ParentAttributeRangeValue = AttributeVal.Name + "#Min" + AttributeVal.RangeOfValues[i].ToString() + "#Max" + AttributeVal.RangeOfValues[j].ToString(); //+ "\n" + "#AttributesParent" + GrandParentValue;
                        BeforeCreatingSubparts(AttributeVal.Name, AttributeVal.RangeOfValues[j], AttributeVal.RangeOfValues[i], "NotFromFormLoad", ParentAttributes);
                        //FindingInfoGain(RefiningDiseasePresentAndNotPresentDataSet(HeartDiseasePresent, AttributeVal.Name, AttributeVal.RangeOfValues[j], AttributeVal.RangeOfValues[i]), RefiningDiseasePresentAndNotPresentDataSet(HeartDiseaseNotPresent, AttributeVal.Name, AttributeVal.RangeOfValues[j], AttributeVal.RangeOfValues[i]));
                        FindingInfoGain(RefiningDiseasePresentAndNotPresentDataSet(TempHeartDiseasePresentForStoringParentNaNObjects, AttributeVal.Name), RefiningDiseasePresentAndNotPresentDataSet(TempHeartDiseaseNotPresentForStoringParentNaNObjects, AttributeVal.Name));
                        //returningValue.Add(CalculateInfoGainOfSingleAttribute(ParentAttributeMainSubPartsOfAttributesSaving, ParentAttributeRangeValue));
                        MainSubPartsOfAttributesSaving temp = CalculateInfoGainOfSingleAttribute(ParentAttributeMainSubPartsOfAttributesSaving, ParentAttributeRangeValue);
                        returningValue.Add(temp);
                        //tempNode = new TreeNode(temp.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.AttributeName+"MinValue:" + temp.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.MinValue + "MaxValue:"+ temp.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.MaxValue);
                        //treeNodeToBeReturned.Nodes.Add(tempNode);
                    }
                    else
                    {

                        // a bit of process for divding all of the attributes which are with high range of max and min values
                    }
                }
            }
            else
            {
                for (int i = 0; i < AttributeVal.RangeOfValues.Count(); i++)
                {

                    if (AttributeVal.RangeOfValues.Count() > 1)
                    {
                        //tempNode = new TreeNode();
                        //String ParentAttributeRangeValue = AttributeVal.Name + '#' + AttributeVal.RangeOfValues[i].ToString();
                        String ParentAttributeRangeValue = AttributeVal.Name + "#Min" + AttributeVal.RangeOfValues[i].ToString() + "#Max" + AttributeVal.RangeOfValues[i].ToString();// + "\n" + "#AttributesParent" + GrandParentValue;
                        BeforeCreatingSubparts(AttributeVal.Name, AttributeVal.RangeOfValues[i], 0.0F, "NotFromFormLoad", ParentAttributes);
                        //FindingInfoGain(RefiningDiseasePresentAndNotPresentDataSet(HeartDiseasePresent, AttributeVal.Name, AttributeVal.RangeOfValues[i], AttributeVal.RangeOfValues[i]), RefiningDiseasePresentAndNotPresentDataSet(HeartDiseaseNotPresent, AttributeVal.Name, AttributeVal.RangeOfValues[j], AttributeVal.RangeOfValues[i]));
                        FindingInfoGain(RefiningDiseasePresentAndNotPresentDataSet(TempHeartDiseasePresentForStoringParentNaNObjects, AttributeVal.Name), RefiningDiseasePresentAndNotPresentDataSet(TempHeartDiseaseNotPresentForStoringParentNaNObjects, AttributeVal.Name));
                        MainSubPartsOfAttributesSaving temp = CalculateInfoGainOfSingleAttribute(ParentAttributeMainSubPartsOfAttributesSaving, ParentAttributeRangeValue);
                        returningValue.Add(temp);
                        //tempNode = new TreeNode(temp.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.AttributeName + "MinValue" + temp.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.MinValue + "MaxValue:" + temp.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.MaxValue);
                        //treeNodeToBeReturned.Nodes.Add(tempNode);
                    }

                }
            }

            return returningValue;
            //return treeNodeToBeReturned;
            String str = "Shiva";
        }


        //further implementation
        //public void CrawlingThroughOtherLevels()
        //{
        //    try
        //    {
        //        TempLstMainSubPartsOfAttributesSaving = new List<MainSubPartsOfAttributesSaving>();
        //        MainSubPartsOfAttributesSaving MainTree = new MainSubPartsOfAttributesSaving();
        //        MainSubPartsOfAttributesSaving tempMainTree;
        //        MainSubPartsOfAttributesSaving MainObject = LstMainSubPartsOfAttributesSaving[0];
        //        MainSubPartsOfAttributesSaving tempMainObject;
        //        MainObject = Funx(MainObject);
        //        tempMainObject = MainObject;
        //        while (tempMainObject != null)
        //        {
        //            for (int i = 0; i < tempMainObject.ChildAttributeValue.Count; i++)
        //            {
        //                tempMainObject.ChildAttributeValue[i] = Funx(tempMainObject.ChildAttributeValue[i]);

        //            }
        //            tempMainObject = 
        //        }
        //        TOPrint();
        //    }
        //    catch (Exception e)
        //    {
        //        string str = e.StackTrace;
        //    }
        //}

        //public MainSubPartsOfAttributesSaving Funx(MainSubPartsOfAttributesSaving MainObj)
        //{
        //    if (MainObj.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseasePositiveEnd == false && MainObj.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseaseNegitiveEnd == false)
        //    {

        //        SingleAttributeValueAndRange temp = MainObj.SingleAttributeValueAndRangeObject;
        //        String ParentValue = MainObj.ParentAttributeRangeValue;

        //        if (temp.Name == "Age")
        //        {
        //            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 10, "Age");
        //        }

        //        else if (temp.Name == "MaxHeartRate")
        //        {
        //            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 20, "MaxHeartRate");

        //        }

        //        if (temp.Name == "OldPeak")
        //        {
        //            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 1, "OldPeak");
        //        }
        //        else if (temp.Name == "RestingBP")
        //        {
        //            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 10, "RestingBP");
        //        }

        //        if (temp.Name == "SerumCholestoral")
        //        {
        //            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 50, "SerumCholestoral");
        //        }

        //        MainSubPartsOfAttributesSaving tempLstMainSubPartsOfAttributesSaving = MainObj.ParentAttributeValue;
        //        String ParentAttributeRangeValue = null;
        //        while (tempLstMainSubPartsOfAttributesSaving != null)
        //        {
        //            ParentAttributes.Add(tempLstMainSubPartsOfAttributesSaving.SingleAttributeInfoGainValueObject.AttributeName);
        //            tempLstMainSubPartsOfAttributesSaving = tempLstMainSubPartsOfAttributesSaving.ParentAttributeValue;
        //            ParentAttributeRangeValue = ParentAttributeRangeValue + MainObj.ParentAttributeRangeValue;
        //        }
        //        ParentAttributes.Add(temp.Name);

        //        //var returningValue = (List<MainSubPartsOfAttributesSaving>)DivingDataSetOnAttributeMaxInfoGain(MainObj, temp, ParentValue, ParentAttributes);
        //        MainObj.ChildAttributeValue = (List<MainSubPartsOfAttributesSaving>)DivingDataSetOnAttributeMaxInfoGain(MainObj, temp, ParentValue, ParentAttributes);
        //        //tempMainTree = new MainSubPartsOfAttributesSaving();

        //        //}
        //        return MainObj;
        //    }
        //    else
        //    {
        //        TempLstMainSubPartsOfAttributesSaving.Add(MainObj);
        //        return MainObj;
        //    }
        //}

        List<MainSubPartsOfAttributesSaving> TempLstMainSubPartsOfAttributesSaving;
        public void CrawlingThroughOtherLevels()
        {
            try
            {
                TempLstMainSubPartsOfAttributesSaving = new List<MainSubPartsOfAttributesSaving>();
                MainSubPartsOfAttributesSaving MainTree = new MainSubPartsOfAttributesSaving();
                MainSubPartsOfAttributesSaving tempMainTree;
                //TreeNode treenode;
                //TreeView tree = new TreeView();
                for (int i = 0; i < LstMainSubPartsOfAttributesSaving.Count; i++)
                {
                    
                    ParentAttributes = new ArrayList();
                    if (LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseasePositiveEnd == false && LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseaseNegitiveEnd == false)
                    {
                        
                        //if (LstMainSubPartsOfAttributesSaving[i].ReachedEnd == false)
                        //{
                        SingleAttributeValueAndRange temp = LstMainSubPartsOfAttributesSaving[i].SingleAttributeValueAndRangeObject;
                        String ParentValue = LstMainSubPartsOfAttributesSaving[i].ParentAttributeRangeValue;
                        // here for level 0 as it was thal with only 3 values 3, 6, 7 it was okay for sending those values in range
                        //"SingleAttributeValueAndRangeObject.RangeOfValues"
                        //but for all other attributes with has Ranges like age, resting bp etc
                        // for these attributes, you have to process them here and add all of them in the same array.
                        // handling the values should be taken care at "DivingDataSetOnAttributeMaxInfoGain"
                        // by handling I mean, all max and min ranges will be added side my side in same array,
                        // 1st max-min range in the array would be [0],[1] and second max-min range would be [2],[3].
                        // so for this in that method it should be handled well
                        // BUTTTTTTTTTTT here you need to process all of those values and save them in d same array :)
                        //
                        //
                        //
                        // IDEA: create a method in common for dividing attributes values to ranges, parameters should be just maximum and minimum values
                        // return value from that method should be an array with all max-min combinations.
                        // writing this method would be helpful for this process(here in this method) and also will be helpful for BeforeCreatingSubparts
                        //method GOOD GOING TILL NOW!! RAISE IT UP :)
                        //Serum Cholestrol

                        if (temp.Name == "Age")
                        {
                            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 10, "Age");
                        }
                        //else if (temp.Name == "ChestPain")
                        //{
                        //    temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1] - 9, temp.RangeOfValues[0] - 10, 10, "Age");
                        //}
                        //else if (temp.Name == "ExerciseIncludeAngina")
                        //{

                        //}
                        //else if (temp.Name == "FastingBloodSugar")
                        //{

                        //}
                        else if (temp.Name == "MaxHeartRate")
                        {
                            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 20, "MaxHeartRate");

                        }
                        //else if (temp.Name == "NumberOfMajorVessels")
                        //{

                        //}
                        if (temp.Name == "OldPeak")
                        {
                            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 1, "OldPeak");
                        }
                        else if (temp.Name == "RestingBP")
                        {
                            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 10, "RestingBP");
                        }
                        //else if (temp.Name == "RestingElectrocardiographic")
                        //{

                        //}
                        if (temp.Name == "SerumCholestoral")
                        {
                            temp.RangeOfValues = DivdingMaxMinIntoSpecifiedRanges(temp.RangeOfValues[1], temp.RangeOfValues[0], 50, "SerumCholestoral");
                        }
                        //else if (temp.Name == "Sex")
                        //{

                        //}
                        //else if (temp.Name == "SlopeOfThePeakExcercise")
                        //{

                        //}
                        //else if (temp.Name == "Thal")
                        //{

                        //}
                        // List<MainSubPartsOfAttributesSaving> LstMainSubPartsOfAttributesSaving = new List<MainSubPartsOfAttributesSaving>();

                        MainSubPartsOfAttributesSaving tempLstMainSubPartsOfAttributesSaving = LstMainSubPartsOfAttributesSaving[i].ParentAttributeValue;
                        String ParentAttributeRangeValue = null;
                        while (tempLstMainSubPartsOfAttributesSaving != null)
                        {
                            ParentAttributes.Add(tempLstMainSubPartsOfAttributesSaving.SingleAttributeInfoGainValueObject.AttributeName);
                            tempLstMainSubPartsOfAttributesSaving = tempLstMainSubPartsOfAttributesSaving.ParentAttributeValue;
                            ParentAttributeRangeValue = ParentAttributeRangeValue + LstMainSubPartsOfAttributesSaving[i].ParentAttributeRangeValue;
                        }
                        ParentAttributes.Add(temp.Name);
                        
                        //
                        //treenode = new TreeNode(LstMainSubPartsOfAttributesSaving[i].SingleAttributeValueAndRangeObject.Name);
                            //
                        List<MainSubPartsOfAttributesSaving> returningValue;
                        returningValue = DivingDataSetOnAttributeMaxInfoGain(LstMainSubPartsOfAttributesSaving[i], temp, ParentValue, ParentAttributes);
                        LstMainSubPartsOfAttributesSaving[i].ChildAttributeValue = returningValue;
                        // check with this, for every iteration of for loop you have to send next level of treenodes,
                        //you'll be returned a treenode which is the treenode you sent as input parameter with its childnode added as nodes below it
                        //with about line of code you are replacing the treenode you sent with the treenode you recieve as output
                        //for next iteration you have to send the next level treenode's one at a time 

                        //tree.Nodes.Add(treenode);
                        //treenode = treenode.Nodes[0];
                        //treenode = treenode.Nodes[1];
                        //treenode = treenode.Nodes[2];

                        //tempMainTree = new MainSubPartsOfAttributesSaving();

                        //}
                    }
                    else
                    {
                        TempLstMainSubPartsOfAttributesSaving.Add(LstMainSubPartsOfAttributesSaving[i]);
                    }
                    

                }

                
                var random = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 1);
                var random2 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 2);
                var random3 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 3);
                var random4 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 4);
                var random5 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 5);
                var random6 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 6);
                var random7 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 7);
                var random8 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 8);
                var random9 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 9);
                var random10 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 10);
                var random11= TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 11);
                var random12 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 12);
                var random13 = TempLstMainSubPartsOfAttributesSaving.Select(o => o.LevelNumber = 13);

                    TOPrint();
            }
            catch (Exception e)
            {
                string str = e.StackTrace;
            }
        }

        public String TOPrint()
        {
            try
            {
                //String str = "";
                //for (int i = 0; i < LstMainSubPartsOfAttributesSaving.Count; i++)
                //{
                //    String RangeValue = "";
                //    for (int j = 0; j < LstMainSubPartsOfAttributesSaving[i].SingleAttributeValueAndRangeObject.RangeOfValues.Count(); j++)
                //    {
                //        RangeValue = RangeValue + ',' + LstMainSubPartsOfAttributesSaving[i].SingleAttributeValueAndRangeObject.RangeOfValues[j];
                //    }
                //    str = str + "\n" + "+++++++++++++++++++++++++++++++++++++++++++++++++" + "\n" + "#AttributeName:" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeName + "\n" + "#ParentName#:" + LstMainSubPartsOfAttributesSaving[i].ParentAttributeRangeValue + "\n" + "@@RangeValuesOfSingleAttribute@@:" + RangeValue + "\n" + "HeartDiseasePositiveEnd" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseasePositiveEnd.ToString() + "//" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.PositiveProbOfSingleAttribute.ToString() + "\n" + "HeartDiseasePositiveEnd" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseaseNegitiveEnd.ToString() + "//" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.NegitiveProbOfSingleAttribute.ToString();
                //}

                //String ReturnVal = null;
                ////TreeView tr = new TreeView();
                ////TreeNode node;
                ////ActualTreeForRepresentaton Tree = new ActualTreeForRepresentaton();
                ////ActualTreeForRepresentaton tempTree = new ActualTreeForRepresentaton();
                ////44.0  - AGE
                ////1.0 - SEX
                ////2.0 - CHEST PAIN
                ////120.0 - RESTING BLOOD PRESSURE
                ////263.0 - SERUM CHOLESTORAL in MG/DL
                ////0.0 - FASTING BLOOD SUGAR > 120 mg/dl
                ////0.0 - resting electrocardiographic results (values 0,1,2) 
                ////173.0 - maximum heart rate achieved 
                ////0.0 - exercise induced angina 
                ////0.0 - oldpeak = ST depression induced by exercise relative to rest
                ////1.0 - the slope of the peak exercise ST segment 
                ////0.0 - number of major vessels (0-3) colored by flourosopy 
                ////7.0 - thal: 3 = normal; 6 = fixed defect; 7 = reversable 


                //MainSubPartsOfAttributesSaving RootNode = new MainSubPartsOfAttributesSaving();
                //for (int j = 0; j < TempLstMainSubPartsOfAttributesSaving.Count; j++)
                //{
                //    String ParentAttributes = null;
                //    MainSubPartsOfAttributesSaving tempMainSubParts = TempLstMainSubPartsOfAttributesSaving[j];
                //    /////////MainSubPartsOfAttributesSaving tempMainSubPartsForTree = TempLstMainSubPartsOfAttributesSaving[j];
                //    //Tree.AttributeName = tempMainSubParts.SingleAttributeInfoGainValueObject.AttributeName;
                //    while (tempMainSubParts.ParentAttributeValue != null)
                //    {
                //        //tempTree.MaxValue = tempMainSubParts.ParentAttributeValue.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.MaxValue;
                //        //tempTree.MinValue = tempMainSubParts.ParentAttributeValue.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.MinValue;
                //        //tempTree.AttributeName = tempMainSubParts.ParentAttributeValue.SingleAttributeInfoGainValueObject.AttributeName;
                //        ParentAttributes = ParentAttributes + "<-" + tempMainSubParts.ParentAttributeRangeValue;
                //        tempMainSubParts = tempMainSubParts.ParentAttributeValue;
                //        //tempTree.ListOfChildNodes.Add(Tree);
                //    }
                //    //if(tempMainSubParts.ParentAttributeValue == null)
                //    //{
                //    //     RootNode= (MainSubPartsOfAttributesSaving)CopyPropertyValues(tempMainSubParts,RootNode);

                //    //    //RootNode = tempMainSubParts;
                //    //}
                //    //if (RootNode.SingleAttributeInfoGainValueObject.AttributeName == "Thal")
                //    //{
                //    //    float Value = InputParameter.Thal;
                //    //    TempLstMainSubPartsOfAttributesSaving.Find(o => o.)
                //    //}


                //    //node = new TreeNode(tempMainSubParts.SingleAttributeInfoGainValueObject.AttributeName);
                //    //node.
                //    //tr.Nodes.Add(node);
                //    //////while (tempMainSubPartsForTree.ParentAttributeRangeValue != null)
                //    //////{
                //    //////    if (tempMainSubPartsForTree.SingleAttributeInfoGainValueObject.AttributeName == "Age")
                //    //////    {
                //    //////        if (InputParameter.Age <= tempMainSubPartsForTree.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.MaxValue && InputParameter.Age >= tempMainSubPartsForTree.SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.MinValue)
                //    //////        {
                //    //////            tempMainSubPartsForTree = tempMainSubPartsForTree.ParentAttributeValue;
                //    //////        }
                //    //////    }
                //    //////}

                //    ReturnVal = ReturnVal + "\n" + "+++++++++++++++++++" + "\n" + "AttributeName: " + TempLstMainSubPartsOfAttributesSaving[j].SingleAttributeInfoGainValueObject.AttributeName + "\n" + "NegitiveValue: " + TempLstMainSubPartsOfAttributesSaving[j].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseaseNegitiveEnd.ToString() + "##\n NegitiveValue: " + TempLstMainSubPartsOfAttributesSaving[j].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.NegitiveProbOfSingleAttribute + "\n PositiveValue" + TempLstMainSubPartsOfAttributesSaving[j].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseasePositiveEnd.ToString() + "##\n PositiveValue: " + TempLstMainSubPartsOfAttributesSaving[j].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.PositiveProbOfSingleAttribute + "\n" + "ParentHierarchy: " + ParentAttributes + "\n";


                //    //str = str + "\n" + "+++++++++++++++++++++++++++++++++++++++++++++++++" + "\n" + "#AttributeName:" + TempLstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeName + "\n" + "#ParentName#:" + LstMainSubPartsOfAttributesSaving[i].ParentAttributeRangeValue + "\n" + "@@RangeValuesOfSingleAttribute@@:" + RangeValue + "\n" + "HeartDiseasePositiveEnd" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseasePositiveEnd.ToString() + "//" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.PositiveProbOfSingleAttribute.ToString() + "\n" + "HeartDiseasePositiveEnd" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.heartDiseaseNegitiveEnd.ToString() + "//" + LstMainSubPartsOfAttributesSaving[i].SingleAttributeInfoGainValueObject.AttributeRangesAndHeartDiseaseDicisionValuesObj.NegitiveProbOfSingleAttribute.ToString();
                //}

                //
                //var jsonSerialiser = new JavaScriptSerializer();
                //var json = jsonSerialiser.Serialize(aList);
                // var jsonSerialiser = new JsonSerializer();
                // JsonWriter jsonWriter;
                //jsonSerialiser.Serialize(jsonWriter, TempLstMainSubPartsOfAttributesSaving);
                //System.Runtime.Serialization.Json
                //System.Runtime.Serialization.Json.DataContractJsonSerializer json = new System.Runtime.Serialization.Json.DataContractJsonSerializer();
                //System.Runtime.Serialization.Json;
                string json = JsonConvert.SerializeObject(TempLstMainSubPartsOfAttributesSaving[TempLstMainSubPartsOfAttributesSaving.Count - 1]);
                return "";

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public float[] DivdingMaxMinIntoSpecifiedRanges(float max, float min, float RangeForDivision, String AttributeName)
        {

            // Need to change in this to get 2 value range for each attribute
            #region hide for a while
            try
            {

                float MidValue = (max + min) / 2;

                float[] ReturningValues = new float[4];
                ReturningValues[0] = min;
                //ReturningValues[1] = MidValue;
                //ReturningValues[2] = MidValue + 1;
                ReturningValues[3] = max;
                //if (AttributeName == "OldPeak")
                //{
                ReturningValues[1] = MidValue;
                ReturningValues[2] = MidValue + 0.1F;
                //}
                return ReturningValues;
                //for(int i=0;i<ReturningValues.Count();i++)
                //{
                //ReturningValues[i] = 
                //}

                //int NumberOfRangePartitionsRequired = (((int)(max - min)) / ((int)(RangeForDivision))) * 2;
                //float[] ReturningValues = new float[NumberOfRangePartitionsRequired];
                //for (int j = 0, k = 1; j < NumberOfRangePartitionsRequired; j = j + 2, k = k + 2)
                //{
                //    HeartAttackPositiveobj = new DivisionsOfEachPart();
                //    HeartAttackNegitiveobj = new DivisionsOfEachPart();
                //    if (nameOfTheAttribute != "OldPeak")
                //    {
                //        if (AttributeName != "OldPeak")
                //        {
                //            min = min + RangeForDivision;
                //            max = min + (RangeForDivision - 1);

                //        }
                //        else
                //        {
                //            min = min + 1F;
                //            max = min + 0.9F;
                //        }
                //        ReturningValues[j] = min;
                //        ReturningValues[k] = max;
                //    }
                //    else
                //    {
                //        MinVal = MinVal + 1F;
                //        MaxVal = MinVal + 1F;
                //    }
                //}
                //return ReturningValues;
            }
            catch (Exception e)
            {
                return null;
            }
            #endregion

            #region try-catch area
            //try
            //{
            //    if (AttributeName != "OldPeak")
            //    {
            //        if (min % 10.0 == 0)
            //        {
            //            min = min - 1;
            //        }
            //        while (min % 10.0 != 0)
            //        {
            //            min = min - 1;
            //        }

            //        if (max % 10.0 == 0)
            //        {
            //            max = max + 1;
            //        }
            //        while (max % 10.0 != 0)
            //        {
            //            max = max + 1;
            //        }

            //        //float minValofOldPeak = OldPeak[0];
            //        //float maxValofOldPeak = OldPeak[ValueFormaxValForOldPeak];
            //        //minValofOldPeak = (float)Math.Floor(minValofOldPeak);
            //        //maxValofOldPeak = (float)Math.Ceiling(maxValofOldPeak);
            //        //OldPeakValue = creationOfSubParts("OldPeak", minValofOldPeak - 1, maxValofOldPeak - 0.9F, OldPeakHeartDiseasePresent, OldPeakHeartDiseaseNotPresent, (maxValofOldPeak - minValofOldPeak) / 1, 1); // why dividing with 1.0F? as I need the sub parts with range 1.0F
            //        //mainListWithAllSubParts.Add(OldPeakValue);

            //    }

            //    int NumberOfRangePartitionsRequired = (((int)(max - min)) / ((int)(RangeForDivision))) * 2;
            //    float[] ReturningValues = new float[NumberOfRangePartitionsRequired];
            //    for (int j = 0, k = 1; j < NumberOfRangePartitionsRequired; j = j + 2, k = k + 2)
            //    {
            //        //HeartAttackPositiveobj = new DivisionsOfEachPart();
            //        //HeartAttackNegitiveobj = new DivisionsOfEachPart();
            //        //if (nameOfTheAttribute != "OldPeak")
            //        //{
            //        if (AttributeName != "OldPeak")
            //        {
            //            min = min + RangeForDivision;
            //            max = min + (RangeForDivision - 1);

            //        }
            //        else
            //        {
            //            min = min + 1F;
            //            max = min + 0.9F;
            //        }
            //        ReturningValues[j] = min;
            //        ReturningValues[k] = max;
            //        //}
            //        //else
            //        //{
            //        //    MinVal = MinVal + 1F;
            //        //    MaxVal = MinVal + 1F;
            //        //}
            //    }
            //    return ReturningValues;
            //}
            //catch (Exception e)
            //{
            //    return null;
            //}
            #endregion
        }

        public object[] AccessingPropertiesOfAllClasses(String AttributePropName, object RequiredClass)
        {
            String str = RequiredClass.GetType().ToString();

            if (str.Equals("DecisionTableCreation_WinForm.SingleAttributeValueAndRange"))
            {
                SingleAttributeValueAndRange tempObj = (SingleAttributeValueAndRange)RequiredClass;
                if (AttributePropName == tempObj.Name)
                {

                }
            }
            else if (str.Equals("DecisionTableCreation_WinForm.RangesForAllAttributes"))
            {
                RangesForAllAttributes tempObj = (RangesForAllAttributes)RequiredClass;
                float[] ValuesToBeReturned;
                if (AttributePropName == "Age")
                {
                    ValuesToBeReturned = new float[] { tempObj.AgeMinValue, tempObj.AgeMaxValue };
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "ChestPain")
                {
                    ValuesToBeReturned = tempObj.ChestPainValues;
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "ExerciseIncludeAngina")
                {
                    ValuesToBeReturned = tempObj.ExerciseIncludeAnginaValues;
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "FastingBloodSugar")
                {
                    ValuesToBeReturned = tempObj.FastingBloodSugarValues;
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "MaxHeartRate")
                {
                    ValuesToBeReturned = new float[] { tempObj.MaxHeartRateMinVal, tempObj.MaxHeartRateMaxVal };
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "NumberOfMajorVessels")
                {
                    ValuesToBeReturned = tempObj.NumberOfMajorVesselValues;
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                if (AttributePropName == "OldPeak")
                {
                    ValuesToBeReturned = new float[] { tempObj.OldPeakMinVal, tempObj.OldPeakMaxVal };
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "RestingBP")
                {
                    ValuesToBeReturned = new float[] { tempObj.RestingBloodPressureMinVal, tempObj.RestingBloodPressureMaxVal };
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "RestingElectrocardiographic")
                {
                    ValuesToBeReturned = tempObj.RestingElectrocardiographicResultsValues;
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                if (AttributePropName == "SerumCholestoral")
                {
                    ValuesToBeReturned = new float[] { tempObj.SerumCholestoralMinValue, tempObj.SerumCholestoralMaxValue };
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "Sex")
                {
                    ValuesToBeReturned = tempObj.SexValues;
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "SlopeOfThePeakExcercise")
                {
                    ValuesToBeReturned = tempObj.SlopeOfThePeakExcerciseValues;
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
                else if (AttributePropName == "Thal")
                {
                    ValuesToBeReturned = tempObj.ThalValues;
                    object[] returnVal = new object[ValuesToBeReturned.Count()];
                    for (int i = 0; i < returnVal.Count(); i++)
                    {
                        returnVal[i] = ValuesToBeReturned[i];
                    }
                    return returnVal;
                }
            }

            return null;
        }


        //Notes :
        // Clarity of the day every decision tree doesn't suppose to come exactly same.
        //Now you have written "AttributeRangesAndHeartDiseaseDicisionValues" this class and  storing single single attributes range division's pos and neg values
        // anyway, what ever attribute's info gain is high that attribute will become seperating factor(RULE FOR SURE)
        // so while iterating, check whether it has any range value with pos or neg value ending, then end the tree for that chain there. BINGO!!
        //
        //Before sleeping:
        //As now copying class object to another temp class object is giving some weird results,
        //try doing stuff from "http://stackoverflow.com/questions/2624823/copy-values-from-one-object-to-another" this,
        // I guess this should work well
    }

    /// <summary>
    /// This is the attribute class
    /// </summary>
    public class HeartDiseaseDataSet
    {
        //        70.0 - AGE
        //1.0 - SEX
        //4.0 - CHEST PAIN
        //130.0 - RESTING BLOOD PRESSURE
        //322.0 - SERUM CHOLESTORAL in MG/DL
        //0.0 - FASTING BLOOD SUGAR > 120 mg/dl
        //2.0 - resting electrocardiographic results (values 0,1,2) 
        //109.0 - maximum heart rate achieved 
        //0.0 - exercise induced angina 
        //2.4 - oldpeak = ST depression induced by exercise relative to rest
        //2.0 - the slope of the peak exercise ST segment 
        //3.0 - number of major vessels (0-3) colored by flourosopy 
        //3.0 - thal: 3 = normal; 6 = fixed defect; 7 = reversable 
        //2 - Absence (1) or presence (2) of heart disease 
        //        public string FileLocation { get; set; }
        //public DateTime UpdatedDate { get; set; }
        //public long FileSize { get; set; }
        //public string FileType { get; set; }

        public float Age { get; set; }
        public float Sex { get; set; }
        public float ChestPain { get; set; }
        public float RestingBloodPressure { get; set; }
        public float SerumCholestoral { get; set; }
        public float FastingBloodSugar { get; set; }
        public float RestingElectrocardiographicResults { get; set; }
        public float MaxHeartRate { get; set; }
        public float ExerciseIncludeAngina { get; set; }
        public float OldPeak { get; set; }
        public float SlopeOfThePeakExcercise { get; set; }
        public float NumberOfMajorVessels { get; set; }
        public float Thal { get; set; }
        public float HeartAttackPresence { get; set; }
    }

    /// <summary>
    /// This is sub parts defnition class
    /// </summary>
    public class DivisionsOfEachPart
    {
        public String Name { get; set; }

        public float Value { get; set; }

        public bool HeartDisease { get; set; }

        public float MinVal { get; set; }

        public float MaxVal { get; set; }
    }

    /// <summary>
    /// This class is used for Noting down Info Gain of each attribute
    /// </summary>
    public class SingleAttributeInfoGainValue
    {
        public String AttributeName { get; set; }

        public float InformationValue { get; set; }

        public AttributeRangesAndHeartDiseaseDicisionValues AttributeRangesAndHeartDiseaseDicisionValuesObj { get; set; }
    }

    public class SingleAttributeValueAndRange
    {
        public String Name { get; set; }
        public float[] RangeOfValues { get; set; }
    }

    public class RangesForAllAttributes
    {
        public float AgeMaxValue { get; set; }
        public float AgeMinValue { get; set; }
        public float[] SexValues { get; set; }
        //public float SexMinVal { get; set; }
        public float[] ChestPainValues { get; set; }
        //public float ChestPainMinValue { get; set; }
        public float RestingBloodPressureMaxVal { get; set; }
        public float RestingBloodPressureMinVal { get; set; }
        public float SerumCholestoralMaxValue { get; set; }
        public float SerumCholestoralMinValue { get; set; }
        public float[] FastingBloodSugarValues { get; set; }
        //public float FastingBloodSugarMinVal { get; set; }
        public float[] RestingElectrocardiographicResultsValues { get; set; }
        //public float RestingElectrocardiographicResultsMinValue { get; set; }
        public float MaxHeartRateMaxVal { get; set; }
        public float MaxHeartRateMinVal { get; set; }
        public float[] ExerciseIncludeAnginaValues { get; set; }
        //public float ExerciseIncludeAnginaMinValue { get; set; }
        public float OldPeakMaxVal { get; set; }
        public float OldPeakMinVal { get; set; }
        public float[] SlopeOfThePeakExcerciseValues { get; set; }
        //public float SlopeOfThePeakExcerciseMinValue { get; set; }
        public float[] NumberOfMajorVesselValues { get; set; }
        //public float NumberOfMajorVesselsMinVal { get; set; }
        public float[] ThalValues { get; set; }
        //public float ThalMinValue { get; set; }
    }

    public class MainSubPartsOfAttributesSaving
    {
        public List<List<DivisionsOfEachPart>> mainListWithAllSubParts { get; set; }//= new List<List<DivisionsOfEachPart>>();
        public SingleAttributeValueAndRange SingleAttributeValueAndRangeObject { get; set; }
        public SingleAttributeInfoGainValue SingleAttributeInfoGainValueObject { get; set; }
        public MainSubPartsOfAttributesSaving ParentAttributeValue { get; set; }
        public List<MainSubPartsOfAttributesSaving> ChildAttributeValue { get; set; }
        public String ParentAttributeRangeValue { get; set; }
        public bool ReachedEnd { get; set; }
        public float HeartDiseasePositiveValue { get; set; }
        public float HeartDiseaseNegitiveValue { get; set; }
        public int LevelNumber { get; set; }
    }

    public class AttributeRangesAndHeartDiseaseDicisionValues
    {
        public String AttributeName { get; set; }
        public float MaxValue { get; set; }
        public float MinValue { get; set; }
        public float PositiveProbOfSingleAttribute { get; set; }
        public float NegitiveProbOfSingleAttribute { get; set; }
        public Boolean heartDiseasePositiveEnd { get; set; }
        public Boolean heartDiseaseNegitiveEnd { get; set; }

    }

    public class ActualTreeForRepresentaton
    {
        public String AttributeName { get; set; }
        public List<ActualTreeForRepresentaton> ListOfChildNodes { get; set; }
        public float MaxValue { get; set; }
        public float MinValue { get; set; }
        public int RangeinTheTreeStructure { get; set; }
    }
}


//////////////////// //data = new HeartDiseaseDataSet();
//////////////////// float tempVal = 0;
//////////////////// tempVal = HeartDiseaseDataSetList[i].Age;
//////////////////// //tempVal = HeartDiseaseDataSetList[i].Age;
//////////////////////if(AgeMinValue == 0.0 && AgeMaxValue == 0.0)
//////////////////// if(i==0)
//////////////////// {
////////////////////     AgeMinValue = tempVal;
////////////////////     AgeMaxValue = tempVal;

//////////////////// }
//////////////////// if(AgeMinValue > tempVal)
//////////////////// {
////////////////////     AgeMinValue = tempVal;
//////////////////// }
//////////////////// else
//////////////////// {
////////////////////     AgeMaxValue = tempVal;
//////////////////// }
//////////////////// tempVal = 0;
//////////////////// tempVal = HeartDiseaseDataSetList[i].RestingBloodPressure;
//////////////////// //tempVal = HeartDiseaseDataSetList[i].Age;
//////////////////// //if (RestingBpMinValue == 0.0 && RestingBpMaxValue == 0.0)
//////////////////// if(i==0)
//////////////////// {
////////////////////     RestingBpMinValue = tempVal;
////////////////////     RestingBpMaxValue = tempVal;
//////////////////// }
//////////////////// if (RestingBpMinValue < tempVal)
//////////////////// {
////////////////////     RestingBpMaxValue = tempVal;
//////////////////// }
//////////////////// else
//////////////////// {
////////////////////     RestingBpMinValue = tempVal;
//////////////////// }
//////////////////// tempVal = 0;
//////////////////// tempVal = HeartDiseaseDataSetList[i].SerumCholestoral;
//////////////////// //tempVal = HeartDiseaseDataSetList[i].Age;
//////////////////// //if (SerumCholestoralMinValue == 0.0 && SerumCholestoralMaxValue == 0.0)
//////////////////// if(i==0)
//////////////////// {
////////////////////     SerumCholestoralMinValue = tempVal;
////////////////////     SerumCholestoralMaxValue = tempVal;
//////////////////// }
//////////////////// if (SerumCholestoralMinValue < tempVal)
//////////////////// {
////////////////////     SerumCholestoralMaxValue = tempVal;
//////////////////// }
//////////////////// else
//////////////////// {
////////////////////     SerumCholestoralMinValue = tempVal;
//////////////////// }
//////////////////// tempVal = 0;
//////////////////// tempVal = HeartDiseaseDataSetList[i].FastingBloodSugar;
//////////////////// //tempVal = HeartDiseaseDataSetList[i].Age;
////////////////////// if (FastingBloodSugarMinValue == 0.0 && FastingBloodSugarMaxValue == 0.0)
//////////////////// if(i==0)
//////////////////// {
////////////////////     FastingBloodSugarMinValue = tempVal;
////////////////////     FastingBloodSugarMaxValue = tempVal;
//////////////////// }
//////////////////// if (FastingBloodSugarMinValue < tempVal)
//////////////////// {
////////////////////     FastingBloodSugarMaxValue = tempVal;
//////////////////// }
//////////////////// else
//////////////////// {
////////////////////     FastingBloodSugarMinValue = tempVal;
//////////////////// }
//////////////////// tempVal = 0;
//////////////////// tempVal = HeartDiseaseDataSetList[i].MaxHeartRate;
//////////////////// //tempVal = HeartDiseaseDataSetList[i].Age;
//////////////////// //if (MaxHeartRateMinValue == 0.0 && MaxHeartRateMaxValue == 0.0)
//////////////////// if(i==0)
//////////////////// {
////////////////////     MaxHeartRateMinValue = tempVal;
////////////////////     MaxHeartRateMaxValue = tempVal;
//////////////////// }
//////////////////// if (MaxHeartRateMinValue < tempVal)
//////////////////// {
////////////////////     MaxHeartRateMaxValue = tempVal;
//////////////////// }
//////////////////// else
//////////////////// {
////////////////////     MaxHeartRateMinValue = tempVal;
//////////////////// }
//////////////////// tempVal = 0;
//////////////////// tempVal = HeartDiseaseDataSetList[i].OldPeak;
//////////////////// //tempVal = HeartDiseaseDataSetList[i].Age;
////////////////////// if (OldPeakMinValue == 0.0 && OldPeakMaxValue == 0.0)
//////////////////// if(i==0)
//////////////////// {
////////////////////     OldPeakMinValue = tempVal;
////////////////////     OldPeakMaxValue = tempVal;
//////////////////// }
//////////////////// if (OldPeakMinValue < tempVal)
//////////////////// {
////////////////////     OldPeakMaxValue = tempVal;
//////////////////// }
//////////////////// else
//////////////////// {
////////////////////     OldPeakMinValue = tempVal;
//////////////////// }



//public float[] creationOfSubParts(String nameOfTheAttribute, int minVal, int maxVal)
//        {


////////////////////division = new DivisionsOfEachPart();
////////////////////List<DivisionsOfEachPart> singleAttribute = new List<DivisionsOfEachPart>();
////////////////////for (int i = 0; i < HeartDiseasePresent.Count; i++)
////////////////////{
////////////////////    if (nameOfTheAttribute == "Age")
////////////////////    {
////////////////////        if (minVal <= HeartDiseasePresent[i].Age && HeartDiseasePresent[i].Age <= maxVal)
////////////////////        {
////////////////////            division.Name = nameOfTheAttribute;

////////////////////            division.Value = HeartDiseasePresent[i].Age;

////////////////////            division.HeartDisease = HeartDiseasePresent[i].HeartAttackPresence;
////////////////////            singleAttribute.Add(division);
////////////////////        }
////////////////////    }
////////////////////}
////////////////////return singleAttribute;

//RestingBP = new float[HeartDiseaseDataSetList.Count];
//SerunCholestoral = new float[HeartDiseaseDataSetList.Count];
//FastingBloodSugar = new float[HeartDiseaseDataSetList.Count];
//MaxHeartRate = new float[HeartDiseaseDataSetList.Count];
//OldPeak = new float[HeartDiseaseDataSetList.Count];

//if (nameOfTheAttribute == "Age")
//{
//    var someVal = Age1.Where(item => maxVal <= item);// && item <= maxVal);
//    //for (int j = 0; j < Age.Count(); j++)
//    //{
//    //    if (minVal <= Age[j] && HeartDiseasePresent[i].Age <= maxVal)
//    //    {
//    //    }
//    //}
//}
//Array.Sort(RestingBP);
//Array.Sort(SerunCholestoral);
//Array.Sort(FastingBloodSugar);
//Array.Sort(MaxHeartRate);
//Array.Sort(OldPeak);
//return null;
//}
