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

struct LightProperties {
	vec4 Position;
	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
};

uniform LightProperties uLight[3];
//uniform LightProperties uLight;

struct MaterialProperties {
	vec3 AmbientReflectivity;
	vec3 DiffuseReflectivity;
	vec3 SpecularReflectivity;
	float Shininess;
};

uniform MaterialProperties uMaterial;

void main()
{
	/*
	vec4 lightDir = normalize(uLight.Position - oSurfacePosition);
	float diffuseFactor = max(dot(oNormal, lightDir), 0);


	vec4 reflectedVector = reflect(-lightDir, oNormal);

	float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), 30);
	float ambientFactor = 0.05f;

	// Combine the total light
    vec4 totalLight = vec4(uLight.AmbientLight * uMaterial.AmbientReflectivity +
						   uLight.DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor +
						   uLight.SpecularLight * uMaterial.SpecularReflectivity * specularFactor, 1);

	// If no Texture Coords are present
	if (oTexCoords.xy == vec2(0, 0))
		FragColour = totalLight;
	else
		// Check the current texture index
		if (uTextureIndex == 0)
			FragColour = texture(uTextureSampler1, oTexCoords) * totalLight;
		else if (uTextureIndex == 1)
		    FragColour = texture(uTextureSampler2, oTexCoords) * totalLight;
	*/

	for(int i = 0; i < 3; ++i)
	{
		vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);
		vec4 lightDir = normalize(uLight[i].Position - oSurfacePosition);
		vec4 reflectedVector = reflect(-lightDir, oNormal);
		float diffuseFactor = max(dot(oNormal, lightDir), 0);
		float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0),
		uMaterial.Shininess);
		vec4 totalLight = FragColour + vec4(uLight[i].AmbientLight *
		uMaterial.AmbientReflectivity + uLight[i].DiffuseLight * uMaterial.DiffuseReflectivity *
		diffuseFactor + uLight[i].SpecularLight * uMaterial.SpecularReflectivity * specularFactor, 1);

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
}