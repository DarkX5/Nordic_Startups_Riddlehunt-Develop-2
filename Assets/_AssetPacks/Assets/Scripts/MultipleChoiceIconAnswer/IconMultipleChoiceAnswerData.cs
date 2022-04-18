using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UnityEngine;
using Random = System.Random;

namespace Answers.MultipleChoice.Data.Icon
{
    public class IconWithValue
    {
        public string Value { get; set; }
        public bool Correct { get; set; }
        public Sprite Icon { get; set; }
    }

    public class IconMultipleChoiceAnswerData : IAnswerAsset
    {
        public string Url
        {
            get { throw new NotImplementedException(); }
        }
       public class AnswerCollection
        {
            [JsonProperty("type")]
            public MultipleChoiceTypes Type { get; set; }
            
            [JsonProperty("answers")]
            public List<AnswerOption> Answers { get; set; }
        }

        public class AnswerOption
        {
            [JsonProperty("value")]
            public string Value { get; set; }
            
            [JsonProperty("correct")]
            public bool Correct { get; set; }
            [JsonProperty("icon_link")]
            public string IconLink { get; set; }
        }

        public List<IconWithValue> CorrectAnswers { get; private set; }
        public List<IconWithValue> PossibleAnswers { get; private set; }
        public string RecordedAnswer { get; private set; } = null;
        private IHuntSessionPersistor _sessionPersistor;
        public IconMultipleChoiceAnswerData(string identifier, ITextGetter textGetter, IImageGetter imageGetter, string answerAssetUrl, Action<bool> assetReady)
        {
            _myLock = new object();
            try
            {
                textGetter.GetText(answerAssetUrl, false, (json) =>
                {
                    var collection = JsonConvert.DeserializeObject<AnswerCollection>(json);
                    if (collection.Type != MultipleChoiceTypes.Icon)
                        throw new ArgumentException("Wrong type answer data");

                    CorrectAnswers = new List<IconWithValue>();
                    PossibleAnswers = new List<IconWithValue>();
                    _expectedReturns = collection.Answers.Count;
                    _actualReturns = 0;
                    foreach (var answer in collection.Answers)
                    {
                        imageGetter.GetImage(answer.IconLink, false, (icon) =>
                        {
                            var entry = new IconWithValue()
                            {
                                Value = answer.Value,
                                Correct = answer.Correct,
                                Icon = icon
                            };
                            if (answer.Correct)
                                CorrectAnswers.Add(entry);
                            PossibleAnswers.Add(entry);
                            WaitForCompletion(assetReady);
                        });
                    }
                    _sessionPersistor = new HuntSessionPersistor();
                    _sessionId = identifier;
                    if (_sessionPersistor.HasAnswerInSession(_sessionId))
                    {
                        RecordedAnswer = _sessionPersistor.GetStringAnswer(_sessionId);
                    }
                });
            }
            catch
            {
                assetReady(false);
            }
        }

        private readonly object _myLock;
        private int _expectedReturns = -1;
        private int _actualReturns = -1;
        private void WaitForCompletion(Action<bool> assetReady)
        {
            lock (_myLock)
            {
                _actualReturns++;
                if (_actualReturns == _expectedReturns)
                {
                    System.Random rnd = new Random();
                    PossibleAnswers = new List<IconWithValue>(PossibleAnswers.OrderBy(item => rnd.Next()));
                    assetReady.Invoke(true);
                }
            }
        }

        //this constructor serves to make testing simpler, so that we don't have to write a ton of mocking code.
        public IconMultipleChoiceAnswerData(string identifier, List<IconWithValue> correctAnswers, List<IconWithValue> incorrectAnswers, Action<bool> assetReady)
        {
            CorrectAnswers = correctAnswers;
            PossibleAnswers = incorrectAnswers;
            PossibleAnswers.AddRange(correctAnswers);
            System.Random rnd = new Random();
            PossibleAnswers = new List<IconWithValue>(PossibleAnswers.OrderBy(item => rnd.Next()));
            _sessionPersistor = new HuntSessionPersistor();
            _sessionId = identifier;
            if (_sessionPersistor.HasAnswerInSession(_sessionId))
            {
                RecordedAnswer = _sessionPersistor.GetStringAnswer(_sessionId);
            }
            assetReady(true);
        }
        
        public AnswerType GetAnswerType()
        {
            throw new NotImplementedException();
        }

        private string _sessionId;
        public void SetHuntSessionPersistor(IHuntSessionPersistor huntSessionPersistor, string sessionId)
        {
            _sessionId = sessionId;
            _sessionPersistor = huntSessionPersistor;
            if (_sessionPersistor.HasAnswerInSession(sessionId))
            {
                RecordedAnswer = _sessionPersistor.GetStringAnswer(sessionId);
            }
        }

        public void FulfillAnswer()
        {
            RecordedAnswer = "";
            foreach (var answer in CorrectAnswers)
            {
                RecordedAnswer += answer + ";";
            }
        }
        
        public bool HasAnswer()
        {
            return !string.IsNullOrEmpty(RecordedAnswer);
        }
        
        public bool HasCorrectAnswer()
        {        
            if (RecordedAnswer == null)
                return false;
            if (RecordedAnswer.EndsWith(";"))
                RecordedAnswer = RecordedAnswer.Remove(RecordedAnswer.Length - 1, 1);
            
            var individualAnswers = RecordedAnswer.Split(';');
            if (individualAnswers.Length != CorrectAnswers.Count)
                return false;
            
            var idx = 0;
            foreach (string answer in individualAnswers)
            {
                if (idx != -1)
                    idx = CorrectAnswers.FindIndex(x => x.Value == answer);
                else return false;
            }
            return idx != -1;
        }

        public void ClearSession()
        {
            RecordedAnswer = null;
            _sessionPersistor.ClearAnswerInSession(_sessionId);
        }

        public Task DownloadForOffline()
        {
            throw new NotImplementedException();
        }

        public AssetType Type { get; }

        public void SetAnswer(string answer)
        {
            RecordedAnswer = answer;
            _sessionPersistor.SetMultipleChoiceAnswerIconsString(_sessionId, RecordedAnswer);
        }
    }
}