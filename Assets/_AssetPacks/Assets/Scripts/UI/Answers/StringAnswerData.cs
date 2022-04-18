using System;
using System.Threading.Tasks;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;

public class StringAnswerData : IAnswerAsset
{
    public string Url
    {
        get { throw new NotImplementedException(); }
    }
    /// <summary>
    /// Answer supplied from the CDN to compare against.
    /// </summary>
    public string CorrectAnswer { get; private set; }

    /// <summary>
    /// Answer given by the user and recorded by the system.
    /// </summary>
    public string RecordedAnswer { get; private set; } = null;

    private string _sessionId;
    private HuntSessionPersistor _sessionPersistor;
    /// <summary>
    /// Instantiates with a link to the answer file.
    /// This constructor will retrieve the asset and unpack the data appropriately.
    /// </summary>
    /// <param name="textGetter">The getter to use for downloading the asset.</param>
    /// <param name="answerAssetUrl">The url to the answer file.</param>
    /// <param name="assetReady">The delegate action to invoke after completing succesfully/unsuccesfully.</param>
    public StringAnswerData(string sessionId, ITextGetter textGetter, string answerAssetUrl, Action<bool> assetReady)
    {
        try
        {
            textGetter.GetText(answerAssetUrl, false, (answer) => { 
                CorrectAnswer = answer;
                assetReady(true);
            });

            _sessionId = sessionId;
            _sessionPersistor = new HuntSessionPersistor();
            if (_sessionPersistor.HasAnswerInSession(_sessionId))
            { 
                RecordedAnswer = _sessionPersistor.GetStringAnswer(_sessionId);
            }
        }
        catch
        {
            assetReady(false);
        }
    }

    public StringAnswerData(string sessionId, string correctAnswer)
    {
        CorrectAnswer = correctAnswer;
        _sessionId = sessionId;
        _sessionPersistor = new HuntSessionPersistor();
        if (_sessionPersistor.HasAnswerInSession(_sessionId))
        { 
            RecordedAnswer = _sessionPersistor.GetStringAnswer(_sessionId);
        }
    }
    
    public AnswerType GetAnswerType()
    {
        return AnswerType.InputTextfield;
    }

    public void SetHuntSessionPersistor(IHuntSessionPersistor huntSessionPersistor, string sessionId)
    {
        throw new NotImplementedException();
    }

    public bool HasAnswer()
    {
        //null object pattern, if it's not set then no answer has been given.
        return !string.IsNullOrEmpty(RecordedAnswer);
    }
    
    public bool HasCorrectAnswer()
    {
        return String.Equals(CorrectAnswer, RecordedAnswer, StringComparison.CurrentCultureIgnoreCase);
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
        _sessionPersistor.SetStringAnswer(_sessionId, RecordedAnswer);
    }
}
