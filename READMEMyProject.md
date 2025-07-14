# CodeGeneratorOpenness
Siemens TIA Portal Code Generator via Openness Interface

Since we are doing import of Graph step sequence through an Excel sheet using Openness, I was thinking of building a "structure" code generator for the TIA portal. Right now, most important functions are working so it would be possible to
<br>
-build a group tree<br>
-import any kind of blocks like FB, FC, OB or data types<br>
-generate step sequences as graph (not 100% complete yet) - no config right now

This covers mainly the function we can find in the Openness scripter. But I want to go a little further to build structures more variable. Not sure where it leads to but my goal will be to use templates to build FB with n-times valve FBs through a scripter or later on maybe a tool to configure this.

Based on the description found here:

https://cache.industry.siemens.com/dl/files/886/109826886/att_1163875/v1/TIAPortalOpenness_enUS_en-US.pdf



This version is based on TIA V19.


