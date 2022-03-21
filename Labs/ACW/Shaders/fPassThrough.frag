#version 330

uniform sampler2D uTextureSampler;

in vec4 oColour;
in vec2 oTexCoords;

out vec4 FragColour;

void main()
{
	if (oTexCoords.xy == vec2(0, 0))
		FragColour = oColour;
	else
		FragColour = texture(uTextureSampler, oTexCoords);
		
}