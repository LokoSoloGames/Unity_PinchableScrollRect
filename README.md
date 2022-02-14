# Pinchable ScrollRect for Unity
Pinchable ScrollRect allows users to zoom in and out on the ScrollRect with both touches pinching input or mouse scroll input. 

[![openupm](https://img.shields.io/npm/v/com.lokosolo.pinchable-scrollrect?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.lokosolo.pinchable-scrollrect/)

## Features
* Customizable pinching behaviour with inherited Unity ScrollRect behaviour.
* Overridden OnScroll behaviour for mouse scroll to perform zooming behaviour.
* Customized pinching gesture to perform the same zooming behaviour as mouse scroll input for touches.
* Transformed standard Unity Event IPointerUp & IPointerDown interface into IPinch interface for further customization of touch behaviour

## Getting Started
* Use it same as the way you use the Unity ScrollRect component.

## Important Remarks
* PinchInputDetector component must have a higher execution order than PinchableScrollRect component or any IPinchHandler component in order to consume the original OnDrag pointer event beforehand.
* Without PinchInputDetector, the ScrollRect can still perform zooming with mouse scroll input but not touch input.
* The absolute minimum scale of the Content of the ScrollRect is 1. A customizable lower bound and upper bound of the scale can be set on inspector.
* Any third touches detected after pinching will be ignore, even after both pinching touches are lifted.
* Editor script is needed in order to serialize the extra fields added to the PinchableScrollRect component.

## License
This plugin is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
