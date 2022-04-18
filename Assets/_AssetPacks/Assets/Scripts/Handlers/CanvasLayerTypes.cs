    using Riddlehunt.Beta.Environment.Controls;
    using UnityEngine;
    using UnityEngine.Serialization;

    public interface ICanvasLayerTypes
    {
        public IVideoCanvasController CreateVideoCanvasController();
        public IBetaTesterController CreateIBetaTesterCanvasController();
        public IWebviewCanvas CreateWebviewCanvas();
        public ILoaderView CreateLoaderView();
    }
    public class CanvasLayerTypes : MonoBehaviour, ICanvasLayerTypes
    {
        [SerializeField] private VideoCanvasController videoCanvasControllerPrefab;
        public IVideoCanvasController CreateVideoCanvasController()
        {
            return VideoCanvasController.Factory(videoCanvasControllerPrefab);
        }

        [SerializeField] private BetaTesterCanvasController betaTesterCanvasControllerPrefab;
        public IBetaTesterController CreateIBetaTesterCanvasController()
        {
            return BetaTesterCanvasController.Factory(betaTesterCanvasControllerPrefab);
        }

        [SerializeField] private WebviewCanvas webviewCanvas;

        public IWebviewCanvas CreateWebviewCanvas()
        {
            return WebviewCanvas.Factory(webviewCanvas);
        }
        
        [SerializeField] private LoaderView loaderViewPrefab;

        public ILoaderView CreateLoaderView()
        {
            return LoaderView.Factory(loaderViewPrefab, null);
        }
    }