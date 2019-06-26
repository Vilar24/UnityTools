# UnityTools
A collection of tools for Unity development

## Blendshape Splitter
An asset post processor designed to remove the need for left and right variants of blendshapes.

### Usage
Simply name your Blendshape **[BlendshapeName]LeftRight[BlendPercent]** where:

**BlendshapeName** is anything you like
**BlendPercent** is the distance you would like them to blend in the center as a percentage of the model width

example:
**EyebrowsUpLeftRight4**
will generate:
**EyebrowsUpLeft**
**EyebrowsUpRight**
with 4% of the center width blended
