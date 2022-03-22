#version 330

uniform sampler2D uTextureSampler1;
uniform sampler2D uTextureSampler2;

uniform int uTextureIndex;

in vec4 oColour;
in vec2 oTexCoords;

out vec4 FragColour;

void main()
{
	if (oTexCoords.xy == vec2(0, 0))
		FragColour = oColour;
	else
		if (uTextureIndex == 0)
			FragColour = texture(uTextureSampler1, oTexCoords);
		else if (uTextureIndex == 1)
		    FragColour = texture(uTextureSampler2, oTexCoords);		
}