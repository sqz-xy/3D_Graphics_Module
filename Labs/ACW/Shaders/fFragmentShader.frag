﻿#version 330

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
	FragColour = vec4(0.0);
	for(int i = 0; i < 3; ++i)
	{
		vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);
		vec4 lightDir = normalize(uLight[i].Position - oSurfacePosition);
		vec4 reflectedVector = reflect(-lightDir, oNormal);

		float dist = distance(uLight[i].Position, oSurfacePosition);
		float attenuation = (1.0 / (1.0 + 0.1 * dist + 0.01 * dist * dist));

		float diffuseFactor = max(dot(oNormal, lightDir), 0);
		float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess);

		vec3 ambientLight = uLight[i].AmbientLight * uMaterial.AmbientReflectivity;
		vec3 diffuseLight = uLight[i].DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor;
		vec3 specularLight = uLight[i].SpecularLight * uMaterial.SpecularReflectivity * specularFactor;

		vec4 totalLight = FragColour + vec4(ambientLight + diffuseLight + specularLight, 1);
		vec4 totalLightAtten = totalLight * attenuation;

		// If no Texture Coords are present
	if (oTexCoords.xy == vec2(0, 0))
		FragColour = totalLightAtten;
	else
		// Check the current texture index
		if (uTextureIndex == 0)
			FragColour = texture(uTextureSampler1, oTexCoords) * totalLightAtten;
		else if (uTextureIndex == 1)
		    FragColour = texture(uTextureSampler2, oTexCoords) * totalLightAtten;
	}			
}