using Google.Play.Review;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class IARManager : Singleton<IARManager>
    {
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;
        private bool isHaveFailRequest;
        void Start()
        {
            _reviewManager = new ReviewManager();
        }

        public IEnumerator RequestReview()
        {

            isHaveFailRequest = false;
            if (_reviewManager == null)
                _reviewManager = new ReviewManager();
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                isHaveFailRequest = true;
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();
        }

        public IEnumerator LauchReview()
        {

            if (isHaveFailRequest)
            {

                Application.OpenURL("market://details?id=" + Application.identifier);
            }
            else
            {

                var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
                yield return launchFlowOperation;
                _playReviewInfo = null; // Reset the object
                if (launchFlowOperation.Error != ReviewErrorCode.NoError)
                {

                    // Log error. For example, using requestFlowOperation.Error.ToString().
                    Application.OpenURL("market://details?id=" + Application.identifier);
                    yield break;
                }

            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
        }

        public IEnumerator RateImmediatel()
        {
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Application.OpenURL("market://details?id=" + Application.identifier);
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Application.OpenURL("market://details?id=" + Application.identifier);
                yield break;
            }
        }
    }
}
