using System.Threading.Tasks;
using UnityEngine;

public interface IViewCollector
{
    Task<IStoryComponent> StoryView(string assetName);
    Task<IRiddleTabComponent> RiddleView(string assetName);
    Task<IWispContainerAnswerView> AnswerView(string assetName);
}

public class ViewCollector : IViewCollector
{
    private IAddressableAssetLoader _addressableAssetLoader;

    public ViewCollector(IAddressableAssetLoader addressableAssetLoader)
    {
        _addressableAssetLoader = addressableAssetLoader;
    }

    public async Task<IStoryComponent> StoryView(string assetName)
    {
        Debug.Log("story DL:"+ assetName);
        var storyPrefab = await _addressableAssetLoader.DownloadGameobject(assetName);
        Debug.Log("story:"+ storyPrefab.name);
        return StoryComponent.Factory(storyPrefab);
    }

    public async Task<IRiddleTabComponent> RiddleView(string assetName)
    {
        Debug.Log("riddle DL:"+ assetName);
        var riddlePrefab = await _addressableAssetLoader.DownloadGameobject(assetName);
        Debug.Log("story:"+ riddlePrefab.name);
        return RiddleTabComponent.Factory(riddlePrefab.GetComponent<RiddleTabComponent>(), null);
    }

    public async Task<IWispContainerAnswerView> AnswerView(string assetName)
    {
        var answerPrefab = await _addressableAssetLoader.DownloadGameobject(assetName);
        return WispContainerAnswerView.Factory(answerPrefab, null);
    }
}
