#version 330

uniform sampler2D uTextureSampler;
in vec2 oTexCoords;
out vec4 FragColour;

void main()
{
	FragColour = texture(uTextureSampler, oTexCoords);
}