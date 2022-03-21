#version 330

uniform sampler2D uTextureSampler;

in vec4 oColour;
in vec2 oTexCoords;

out vec4 FragColour;

void main()
{
	if (oTexCoords.xy != 0)
		FragColour = texture(uTextureSampler, oTexCoords);
	else
		FragColour = oColour;
}