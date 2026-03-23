using UnityEngine;

public class GizmoSection : TTObject
{
    public override void InitStaticProperties()
    {
        AddProperty(new StringProperty("Section Title", GizmoSectionLoader.CurrentLoadingSection, StringProperty.MaxSize.Int) { generateOptions = TTProperty.FieldGenerateOptions.Hidden});
        AddProperty(new ObjectSizeProperty(this) { generateOptions = TTProperty.FieldGenerateOptions.Hidden});
    }
}
