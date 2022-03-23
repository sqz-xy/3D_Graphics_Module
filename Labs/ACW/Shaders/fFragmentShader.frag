#version 330

uniform sampler2D uTextureSampler1;
uniform sampler2D uTextureSampler2;
uniform vec4 uLightPosition;

uniform int uTextureIndex;

in vec4 oNormal;
in vec4 oSurfacePosition;
in vec2 oTexCoords;

out vec4 FragColour;

void main()
{
	vec4 lightDir = normalize(uLightPosition - oSurfacePosition);
	float diffuseFactor = max(dot(oNormal, lightDir), 0);
	vec4 totalLight = vec4(vec3(diffuseFactor), 1);

	// If no Texture Coords are present
	if (oTexCoords.xy == vec2(0, 0))
		FragColour = totalLight;
	else
		// Check the current texture index
		if (uTextureIndex == 0)
			FragColour = texture(uTextureSampler1, oTexCoords) * totalLight;
		else if (uTextureIndex == 1)
		    FragColour = texture(uTextureSampler2, oTexCoords) * totalLight;		
}