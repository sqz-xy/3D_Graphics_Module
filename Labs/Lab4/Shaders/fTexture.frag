#version 330

in vec2 oTexCoords;
out vec4 FragColour;

void main()
{
	FragColour = vec4(oTexCoords, 1, 1);
}