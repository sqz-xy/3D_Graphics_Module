#version 330

uniform vec4 uColour;

out vec4 FragColour;

void main()
{
	FragColour = uColour;
}