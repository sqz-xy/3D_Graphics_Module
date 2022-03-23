#version 330

uniform sampler2D uTextureSampler1;
uniform sampler2D uTextureSampler2;
uniform vec4 uLightPosition;
uniform vec4 uEyePosition;

uniform int uTextureIndex;

in vec4 oNormal;
in vec4 oSurfacePosition;
in vec2 oTexCoords;

out vec4 FragColour;

void main()
{
	vec4 lightDir = normalize(uLightPosition - oSurfacePosition);
	float diffuseFactor = max(dot(oNormal, lightDir), 0);

	vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);
	vec4 reflectedVector = reflect(-lightDir, oNormal);

	vec4 pointLight = vec4(vec3(diffuseFactor), 1);
	float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), 30);

	// If no Texture Coords are present
	if (oTexCoords.xy == vec2(0, 0))
		FragColour = pointLight;
	else
		// Check the current texture index
		if (uTextureIndex == 0)
			FragColour = texture(uTextureSampler1, oTexCoords) * pointLight;
		else if (uTextureIndex == 1)
		    FragColour = texture(uTextureSampler2, oTexCoords) * pointLight;		
}