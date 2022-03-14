#version 330

in vec4 oColour;

out vec4 FragColour;

void main()
{
	//vec4 colour = vec4(10, 0, 0, 0);
	FragColour = oColour;
}