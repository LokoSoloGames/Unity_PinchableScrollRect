# Unity UI 可縮放滾動矩形(Pinchable ScrollRect)
可縮放滾動矩形讓用家能夠以手指觸控或滑鼠滾輪來對用戶界面(UI)進行縮放。

[![openupm](https://img.shields.io/npm/v/com.lokosolo.pinchable-scrollrect?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.cn/packages/com.lokosolo.pinchable-scrollrect/)

## 特點
* 可執行自定義的捏合行為與繼承的 Unity滾動矩形(ScrollRect)行為。
* 覆蓋鼠標滾動以執行縮放行為的 OnScroll 行為。
* 自定義捏合手勢以執行與鼠標滾動輸入相同的縮放行為。
* 將標準 Unity Event IPointerUp & IPointerDown 接口轉換為 IPinch 接口，以進一步自定義觸摸行為。

## 入門
* 使用方式與Unity 滾動矩形(ScrollRect) 的方式相同。

## 備注
* PinchInputDetector 組件必須具有比 PinchableScrollRect 組件或任何 IPinchHandler 組件更高的執行順序，以先消耗OnDrag 指針事件。
* 即使沒有 PinchInputDetector組件，ScrollRect 仍然可以通過鼠標滾動進行縮放。
* ScrollRect 內容的絕對最小比例為 1。可在編輯器上設定縮放的比例下限和上限。
* 在捏合後檢測到的任何第三次觸摸都將被忽略，即使在兩次捏合觸摸都被解除後也是如此。
* 內附編輯器腳本來顯示PinchableScrollRect組件新增的額外變量。

## 授權
此插件根據MIT License方式授權 - 詳見[授權](LICENSE)。
