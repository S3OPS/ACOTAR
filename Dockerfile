FROM unityci/editor:2022.3.0f1-windows-mono-1

# Set working directory
WORKDIR /workspace

# Copy project files
COPY . /workspace/

# Set Unity license (will be provided via environment variable)
ARG UNITY_LICENSE
ENV UNITY_LICENSE=$UNITY_LICENSE

# Create build directory
RUN mkdir -p /workspace/Build

# Build command will be executed when container runs
CMD ["/bin/bash"]
