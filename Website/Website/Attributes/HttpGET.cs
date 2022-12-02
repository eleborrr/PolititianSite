namespace Political.Attributes;

internal class HttpGET: Attribute
{
    public string MethodURI;

    public HttpGET(string methodUri)
    {
        MethodURI = methodUri;
    }
}